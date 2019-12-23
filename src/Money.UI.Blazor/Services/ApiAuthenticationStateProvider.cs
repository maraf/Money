using Microsoft.AspNetCore.Components.Authorization;
using Money.Events;
using Neptuo;
using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Money.Services
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IEventDispatcher eventDispatcher;
        private readonly HttpClient http;
        private readonly TokenContainer token;
        private readonly Interop interop;

        public ApiAuthenticationStateProvider(IEventDispatcher eventDispatcher, HttpClient http, TokenContainer token, Interop interop)
        {
            Ensure.NotNull(eventDispatcher, "eventDispatcher");
            Ensure.NotNull(http, "http");
            Ensure.NotNull(token, "token");
            Ensure.NotNull(interop, "interop");
            this.eventDispatcher = eventDispatcher;
            this.http = http;
            this.token = token;
            this.interop = interop;
        }

        public async Task<string> GetTokenAsync()
        {
            if (!token.HasValue)
            {
                token.Value = await interop.LoadTokenAsync();
                if (!String.IsNullOrEmpty(token.Value))
                {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                    await eventDispatcher.PublishAsync(new UserSignedIn());
                }
                else
                {
                    http.DefaultRequestHeaders.Authorization = null;
                    await eventDispatcher.PublishAsync(new UserSignedOut());
                }
            }

            return token.Value;
        }

        public async Task SetTokenAsync(string value, bool isPersistent = false)
        {
            if (isPersistent)
                await interop.SaveTokenAsync(value);

            token.Value = value;

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await GetTokenAsync();

            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}
