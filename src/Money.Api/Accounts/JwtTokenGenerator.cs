using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Money.Accounts.Models;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Money.Accounts
{
    public class JwtTokenGenerator
    {
        public static class ClaimTypes
        {
            public const string Expiration = "exp";
            public const string RefreshToken = "RefreshToken";
            public const string TokenName = "TokenName";
        }

        private readonly JwtSecurityTokenHandler tokenHandler;
        private readonly JwtOptions options;

        public JwtTokenGenerator(JwtSecurityTokenHandler tokenHandler, IOptions<JwtOptions> options)
        {
            Ensure.NotNull(tokenHandler, "tokenHandler");
            Ensure.NotNull(options, "options");
            this.tokenHandler = tokenHandler;
            this.options = options.Value;
        }

        public string Generate(IEnumerable<Claim> claims)
        {
            var credentials = new SigningCredentials(options.GetSecurityKey(), SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.Add(options.GetExpiry());
            var token = new JwtSecurityToken(
                options.Issuer,
                options.Issuer,
                claims,
                expires: expiry,
                signingCredentials: credentials
            );

            return tokenHandler.WriteToken(token);
        }
    }
}
