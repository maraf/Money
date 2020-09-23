using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Money.Accounts.Models;
using Money.Services;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Money.Accounts.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly JwtOptions configuration;
        private readonly UserManager<User> userManager;
        private readonly JwtSecurityTokenHandler tokenHandler;
        private readonly Json json;

        public UserController(IOptions<JwtOptions> configuration, UserManager<User> userManager, JwtSecurityTokenHandler tokenHandler, Json json)
        {
            Ensure.NotNull(configuration, "configuration");
            Ensure.NotNull(userManager, "userManager");
            Ensure.NotNull(tokenHandler, "tokenHandler");
            Ensure.NotNull(json, "json");
            this.configuration = configuration.Value;
            this.userManager = userManager;
            this.tokenHandler = tokenHandler;
            this.json = json;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            User user = await userManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                if (await userManager.CheckPasswordAsync(user, model.Password))
                {
                    DateTime now = DateTime.Now;
                    if (user.LastSignedAt == null || user.LastSignedAt < now)
                    {
                        user.LastSignedAt = now;
                        await userManager.UpdateAsync(user);
                    }

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    };

                    var credentials = new SigningCredentials(configuration.GetSecurityKey(), SecurityAlgorithms.HmacSha256);
                    var expiry = DateTime.Now.Add(configuration.GetExpiry());

                    var token = new JwtSecurityToken(
                        configuration.Issuer,
                        configuration.Issuer,
                        claims,
                        expires: expiry,
                        signingCredentials: credentials
                    );

                    var response = new LoginResponse()
                    {
                        Token = tokenHandler.WriteToken(token)
                    };

                    return Content(json.Serialize(response), "text/json");
                }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            var user = new User(model.UserName, DateTime.Now);
            var result = await userManager.CreateAsync(user, model.Password);

            var response = new RegisterResponse();
            if (!result.Succeeded)
                response.ErrorMessages.AddRange(result.Errors.Select(e => e.Description));

            return Content(json.Serialize(response), "text/json");
        }
    }
}
