using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Money.Controllers
{
    [Route("api/user/login")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        public class LoginModel
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        private readonly IConfiguration configuration;

        public UserLoginController(IConfiguration configuration)
        {
            Ensure.NotNull(configuration, "configuration");
            this.configuration = configuration.GetSection("Jwt");
        }

        [HttpPost]
        public IActionResult Post([FromBody] LoginModel model)
        {
            if (model.UserName == "admin" && model.Password == "admin")
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, model.UserName)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiry = DateTime.Now.AddDays(Convert.ToInt32(configuration["ExpiryInDays"]));

                var token = new JwtSecurityToken(
                    configuration["Issuer"],
                    configuration["Issuer"],
                    claims,
                    expires: expiry,
                    signingCredentials: creds
                );

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                string serializedToken = handler.WriteToken(token);

                return base.Ok(new { token = serializedToken });
            }

            return BadRequest("Username and password are invalid.");
        }
    }
}
