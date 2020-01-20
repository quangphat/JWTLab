using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JWT.Infrastructures
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOption _option;
        public TokenProviderMiddleware(RequestDelegate request, IOptions<TokenProviderOption> option)
        {
            _next = request;
            _option = option.Value;
        }
        public Task Invoke(HttpContext context)
        {
            if(!context.Request.Path.Equals(_option.Path,StringComparison.Ordinal))
            {
                return _next(context);
            }
            return null;
        }
        public async Task GenerateToken(HttpContext context)
        {
            var userName = context.Request.Form["username"];
            var password = context.Request.Form["password"];
            var identity = await GetIdentity(userName, password);
            if(identity==null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid");
                return;
            }
            var now = DateTime.Now;
            var jwt = new JwtSecurityToken(
                claims:identity.Claims,
                notBefore:now,
                expires:now.Add(_option.Expiration),
                signingCredentials:_option.SigningCredentials
                );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_option.Expiration.TotalSeconds
            };
            context.Response.ContentType = "applicatio/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }
        private Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            var user = AccountInMemory.ArrayAccount.FirstOrDefault(p => p.UserName.Equals(username) && p.Password.Equals(password));
            if (user == null)
                return null;
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id, null, ClaimsIdentity.DefaultIssuer, "Providers"));
            claims.Add(new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"));
            claims.Add(new Claim("UserName", user.UserName));
            return Task.FromResult(new ClaimsIdentity(claims, "Bearer"));
        }
    }
}
