using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Accounts.Models
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string SecurityKey { get; set; }
        public int ExpiryInDays { get; set; }

        private SecurityKey securityKey;

        public SecurityKey GetSecurityKey()
        {
            if (securityKey == null)
                securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));

            return securityKey;
        }

        public TimeSpan GetExpiry() => TimeSpan.FromDays(ExpiryInDays);
    }
}
