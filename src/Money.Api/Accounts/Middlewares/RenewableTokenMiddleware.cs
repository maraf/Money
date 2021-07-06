using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Money.Accounts.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Money.Accounts.Middlewares
{
    public class RenewableTokenMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string requestRefreshToken = context.User.FindFirst("RefreshToken")?.Value;

            if (!String.IsNullOrEmpty(requestRefreshToken) && IsNewTokenRequired(context))
            {
                var userManager = context.RequestServices.GetRequiredService<UserManager<User>>();
                var user = await userManager.GetUserAsync(context.User);
                if (user != null)
                {
                    string tokenName = context.User.FindFirst(JwtTokenGenerator.ClaimTypes.TokenName)?.Value;
                    if (!String.IsNullOrEmpty(tokenName))
                    {
                        var validRefreshToken = await userManager.GetAuthenticationTokenAsync(user, JwtTokenGenerator.ClaimTypes.RefreshToken, tokenName);
                        if (requestRefreshToken == validRefreshToken)
                        {
                            var tokenGenerator = context.RequestServices.GetRequiredService<JwtTokenGenerator>();
                            var token = tokenGenerator.Generate(context.User.Claims);

                            context.Response.Headers.Add(RenewalTokenHeader.Name, token);
                        }
                    }
                }
            }

            await next(context);
        }

        protected bool IsNewTokenRequired(HttpContext context)
        {
            if (Int64.TryParse(context.User.FindFirst(JwtTokenGenerator.ClaimTypes.Expiration)?.Value, out var timestamp))
            {
                var jwtOptions = context.RequestServices.GetRequiredService<IOptions<JwtOptions>>().Value;
                var duration = jwtOptions.GetExpiry();

                var currentExpiration = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
                var newExpiration = DateTime.Now.Add(duration);
                var diff = newExpiration - currentExpiration;

                if (diff > duration / 2)
                    return true;
            }

            return false;
        }
    }
}
