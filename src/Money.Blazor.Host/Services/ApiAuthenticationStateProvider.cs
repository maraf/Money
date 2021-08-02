using Microsoft.AspNetCore.Components.Authorization;
using Money.Events;
using Neptuo;
using Neptuo.Events;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Money.Services
{
    public partial class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IEventDispatcher events;
        private readonly HttpClient http;
        private readonly TokenContainer token;
        private readonly TokenStorage tokenStorage;
        private readonly ILog log;

        private ClaimsPrincipal principal;
        private Task loadTokenTask;
        private readonly List<ITokenValidator> validators = new List<ITokenValidator>();

        public ApiAuthenticationStateProvider(IEventDispatcher events, HttpClient http, TokenContainer token, TokenStorage tokenStorage, ILogFactory logFactory, IEnumerable<ITokenValidator> validators)
        {
            Ensure.NotNull(events, "eventDispatcher");
            Ensure.NotNull(http, "http");
            Ensure.NotNull(token, "token");
            Ensure.NotNull(tokenStorage, "tokenStorage");
            Ensure.NotNull(logFactory, "logFactory");
            Ensure.NotNull(validators, "validators");
            this.events = events;
            this.http = http;
            this.token = token;
            this.tokenStorage = tokenStorage;
            this.log = logFactory.Scope("ApiAuthenticationState");
            this.validators.AddRange(validators);
        }

        public ApiAuthenticationStateProvider AddValidator(ITokenValidator validator)
        {
            Ensure.NotNull(validator, "validator");
            validators.Add(validator);
            return this;
        }

        public async Task<string> GetTokenAsync()
        {
            log.Debug("Getting a token.");
            if (!token.HasValue)
            {
                if (loadTokenTask == null)
                {
                    log.Debug("Loading the token from a storage.");

                    loadTokenTask = LoadAndApplyTokenAsync();
                }

                await loadTokenTask;
                loadTokenTask = null;
            }

            log.Debug($"Returning a token '{token.Value}'.");
            return token.Value;
        }

        private async Task LoadAndApplyTokenAsync()
        {
            var value = await tokenStorage.FindAsync();
            await ChangeTokenAsync(value);
        }

        private async Task ChangeTokenAsync(string value, bool isValidationRequired = true, bool isNoficationEnabled = false)
        {
            principal = null;
            token.Value = value;
            if (!String.IsNullOrEmpty(token.Value))
            {
                log.Debug("Applying token.");

                // It is required to set the token here, because validators can use HttpClient to validate the token.
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                bool isValid = true;
                if (isValidationRequired)
                {
                    if (!await ValidateTokenAsync(token.Value))
                    {
                        log.Debug("Token isn't valid.");
                        isValid = false;
                    }
                }

                if (isValid)
                {
                    log.Debug("Token validation succeeded.");

                    EnsurePrincipal(token.Value);
                    Notify(isNoficationEnabled);

                    await events.PublishAsync(new UserSignedIn());
                    return;
                }
            }
            else
            {
                EnsurePrincipal(null);
            }

            log.Debug("Clearing token.");

            // Now create the Authorization header.
            http.DefaultRequestHeaders.Authorization = null;

            Notify(isNoficationEnabled);

            await events.PublishAsync(new UserSignedOut());

            void Notify(bool isNoficationEnabled)
            {
                if (isNoficationEnabled)
                {
                    log.Debug("NotifyAuthenticationStateChanged.");
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                }
            }
        }

        private void EnsurePrincipal(string token)
        {
            if (principal == null)
            {
                var identity = string.IsNullOrEmpty(token)
                    ? new ClaimsIdentity()
                    : new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");

                log.Debug($"Identity '{identity}' ({identity.IsAuthenticated}) with '{identity.Claims.Count()}' claims.");

                principal = new ClaimsPrincipal(identity);
            }
        }

        private async Task<bool> ValidateTokenAsync(string token)
        {
            foreach (var validator in validators)
            {
                if (!await validator.ValidateAsync(token))
                    return false;
            }

            return true;
        }

        public async Task SetTokenAsync(string value, bool isPersistent = false)
        {
            log.Debug($"Set token '{value}'.");

            if (!String.IsNullOrEmpty(value))
                await tokenStorage.SetAsync(value, isPersistent);
            else
                await tokenStorage.ClearAsync();

            await ChangeTokenAsync(value, false, true);
        }

        public Task ClearTokenAsync()
            => SetTokenAsync(null, true);

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (principal == null)
                EnsurePrincipal(await GetTokenAsync());

            log.Debug($"AuthenticationState '{principal?.Identity}'.");
            return new AuthenticationState(principal);
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
