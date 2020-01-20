using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWT.Infrastructures
{
    public class TokenProviderOption
    {
        public string Path { get; set; } = "/token"; //line 1

        public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(+1); //line 2

        public SigningCredentials SigningCredentials { get; set; }//line 3
    }
}
