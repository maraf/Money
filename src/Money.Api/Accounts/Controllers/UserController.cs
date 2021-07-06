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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Money.Accounts.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly JwtTokenGenerator tokenGenerator;
        private readonly Json json;

        public UserController(UserManager<User> userManager, JwtTokenGenerator tokenGenerator, Json json)
        {
            Ensure.NotNull(userManager, "userManager");
            Ensure.NotNull(tokenGenerator, "tokenGenerator");
            Ensure.NotNull(json, "json");
            this.userManager = userManager;
            this.tokenGenerator = tokenGenerator;
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

                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    };

                    if (model.IsAutoRenewable) 
                    {
                        string refreshToken = GenerateRefreshToken();
                        string tokenName = Guid.NewGuid().ToString();
                        claims.Add(new Claim(JwtTokenGenerator.ClaimTypes.RefreshToken, refreshToken));
                        claims.Add(new Claim(JwtTokenGenerator.ClaimTypes.TokenName, tokenName));
                        await userManager.SetAuthenticationTokenAsync(user, JwtTokenGenerator.ClaimTypes.RefreshToken, tokenName, refreshToken);
                    }

                    var token = tokenGenerator.Generate(claims);

                    var response = new LoginResponse()
                    {
                        Token = token
                    };

                    return Content(json.Serialize(response), "text/json");
                }
            }

            return BadRequest();
        }

        protected string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
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
