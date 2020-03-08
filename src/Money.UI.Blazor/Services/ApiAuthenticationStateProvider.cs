using Microsoft.AspNetCore.Components.Authorization;
using Money.Events;
using Neptuo;
using Neptuo.Events;
using Neptuo.Logging;
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
        private readonly ILog log;

        public ApiAuthenticationStateProvider(IEventDispatcher eventDispatcher, HttpClient http, TokenContainer token, Interop interop, ILogFactory logFactory)
        {
            Ensure.NotNull(eventDispatcher, "eventDispatcher");
            Ensure.NotNull(http, "http");
            Ensure.NotNull(token, "token");
            Ensure.NotNull(interop, "interop");
            Ensure.NotNull(logFactory, "logFactory");
            this.eventDispatcher = eventDispatcher;
            this.http = http;
            this.token = token;
            this.interop = interop;
            this.log = logFactory.Scope("ApiAuthenticationState");
        }

        public async Task<string> GetTokenAsync()
        {
            log.Debug("Getting a token.");
            if (!token.HasValue)
            {
                log.Debug("Loading the token from a storage.");

                var value = await interop.LoadTokenAsync();
                await ChangeTokenAsync(value);
            }

            log.Debug($"Returning a token '{token.Value}'.");
            return token.Value;
        }

        private async Task ChangeTokenAsync(string value)
        {
            token.Value = value;
            if (!String.IsNullOrEmpty(token.Value))
            {
                log.Debug("Token found in the storage.");

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                await eventDispatcher.PublishAsync(new UserSignedIn());
            }
            else
            {
                log.Debug("Token not found in the storage.");

                http.DefaultRequestHeaders.Authorization = null;
                await eventDispatcher.PublishAsync(new UserSignedOut());
            }
        }

        public async Task SetTokenAsync(string value, bool isPersistent = false)
        {
            log.Debug($"Set token '{value}'.");

            if (isPersistent)
                await interop.SaveTokenAsync(value);

            await ChangeTokenAsync(value);

            log.Debug("NotifyAuthenticationStateChanged.");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public Task ClearTokenAsync()
            => SetTokenAsync(null, true);

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await GetTokenAsync();

            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");

            log.Debug($"Identity '{identity}' ({identity.IsAuthenticated}) with '{identity.Claims.Count()}' claims.");
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
