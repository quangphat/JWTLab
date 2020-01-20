using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWT
{
    public class AccountInMemory
    {
        public static IList<Account> ArrayAccount = new List<Account>();

        static AccountInMemory()
        {
            ArrayAccount.Add(new Account
            {
                Id = Guid.NewGuid().ToString("n"),
                FirstName = "Butter",
                LastName = "Ngo",
                UserName = "admin",
                Password = "123456"
            });
        }
    }
}
