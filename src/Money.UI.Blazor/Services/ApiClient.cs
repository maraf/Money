using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Money.Events;
using Money.Models.Api;
using Money.Users.Models;
using Neptuo;
using Neptuo.Events;
using Neptuo.Exceptions.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class ApiClient
    {
        private readonly ApiClientConfiguration configuration;
        private readonly TokenContainer token;
        private readonly HttpClient http;
        private readonly CommandMapper commandMapper;
        private readonly QueryMapper queryMapper;
        private readonly IExceptionHandler exceptionHandler;
        private readonly IEventDispatcher eventDispatcher;
        private readonly Interop interop;

        public ApiClient(IOptions<ApiClientConfiguration> configuration, TokenContainer token, HttpClient http, CommandMapper commandMapper, QueryMapper queryMapper, IExceptionHandler exceptionHandler, IEventDispatcher eventDispatcher, Interop interop)
        {
            Ensure.NotNull(configuration, "configuration");
            Ensure.NotNull(token, "token");
            Ensure.NotNull(http, "http");
            Ensure.NotNull(commandMapper, "commandMapper");
            Ensure.NotNull(queryMapper, "queryMapper");
            Ensure.NotNull(exceptionHandler, "exceptionHandler");
            Ensure.NotNull(eventDispatcher, "eventDispatcher");
            Ensure.NotNull(interop, "interop");
            this.configuration = configuration.Value;
            this.token = token;
            this.http = http;
            this.commandMapper = commandMapper;
            this.queryMapper = queryMapper;
            this.exceptionHandler = exceptionHandler;
            this.eventDispatcher = eventDispatcher;
            this.interop = interop;

            http.BaseAddress = this.configuration.ApiUrl;
            EnsureAuthorization();
        }

        private void ClearAuthorization()
        {
            if (token.HasValue)
            {
                token.Value = null;
                http.DefaultRequestHeaders.Authorization = null;
                interop.SaveToken(null);
                interop.StopSignalR();

                eventDispatcher.PublishAsync(new UserSignedOut());
            }
        }

        private void EnsureAuthorization()
        {
            if (token.HasValue && http.DefaultRequestHeaders.Authorization?.Parameter != token.Value)
            {
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                interop.StartSignalR(configuration.ApiUrl.ToString() + "api", token.Value);

                eventDispatcher.PublishAsync(new UserSignedIn());
            }
        }

        public async Task<bool> LoginAsync(string userName, string password, bool isPermanent)
        {
            LoginResponse response = await http.PostJsonAsync<LoginResponse>(
                "/api/user/login",
                new LoginRequest()
                {
                    UserName = userName,
                    Password = password
                }
            );

            if (!String.IsNullOrEmpty(response.Token))
            {
                token.Value = response.Token;
                EnsureAuthorization();

                if (isPermanent)
                    interop.SaveToken(token.Value);

                return true;
            }

            return false;
        }

        public Task LogoutAsync()
        {
            ClearAuthorization();
            return Task.CompletedTask;
        }

        public async Task<RegisterResponse> RegisterAsync(string userName, string password)
        {
            RegisterResponse response = await http.PostJsonAsync<RegisterResponse>(
                "/api/user/register",
                new RegisterRequest()
                {
                    UserName = userName,
                    Password = password
                }
            );

            return response;
        }

        private Request CreateRequest(Type type, string payload)
            => new Request() { Type = type.AssemblyQualifiedName, Payload = payload };

        public async Task<Response> QueryAsync(Type type, string payload)
        {
            if (!token.HasValue)
            {
                token.Value = await interop.LoadTokenAsync();
                EnsureAuthorization();
            }

            string url = queryMapper.FindUrlByType(type);
            if (url != null)
            {
                try
                {
                    HttpResponseMessage response = await http.PostAsync($"/api/query/{url}", new StringContent(payload, Encoding.UTF8, "text/json"));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return SimpleJson.SimpleJson.DeserializeObject<JsResponse>(responseContent);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        ClearAuthorization();
                        throw new UnauthorizedAccessException();
                    }
                    else if(response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        throw new InternalServerException();
                    }
                    else
                    {
                        throw Ensure.Exception.InvalidOperation($"Generic HTTP error: {response.StatusCode}.");
                    }
                }
                catch (Exception e)
                {
                    if (e is HttpRequestException)
                        e = new ServerNotRespondingException(e);

                    exceptionHandler.Handle(e);
                    throw;
                }
            }
            else
            {
                return await http.PostJsonAsync<Response>($"/api/query", CreateRequest(type, payload));
            }
        }

        public Task CommandAsync(Type type, string payload)
        {
            string url = commandMapper.FindUrlByType(type);
            if (url != null)
                return http.PostJsonAsync($"/api/command/{url}", payload);
            else
                return http.PostJsonAsync($"/api/command", CreateRequest(type, payload));
        }

        public class JsResponse : Response
        {
            public string payload
            {
                set => Payload = value;
            }

            public string type
            {
                set => Type = value;
            }

            public ResponseType responseType
            {
                set => ResponseType = value;
            }
        }
    }
}
