using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Money.Events;
using Money.Models.Api;
using Money.Users.Models;
using Neptuo;
using Neptuo.Events;
using Neptuo.Exceptions.Handlers;
using Neptuo.Logging;
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
        private readonly HttpClient http;
        private readonly CommandMapper commandMapper;
        private readonly QueryMapper queryMapper;
        private readonly IExceptionHandler exceptionHandler;
        private readonly ApiAuthenticationStateProvider authenticationState;
        private readonly ILog log;
        private readonly Json json;

        public ApiClient(IOptions<ApiClientConfiguration> configuration, HttpClient http, CommandMapper commandMapper, QueryMapper queryMapper, IExceptionHandler exceptionHandler, ApiAuthenticationStateProvider authenticationState, ILogFactory logFactory, Json json)
        {
            Ensure.NotNull(configuration, "configuration");
            Ensure.NotNull(http, "http");
            Ensure.NotNull(commandMapper, "commandMapper");
            Ensure.NotNull(queryMapper, "queryMapper");
            Ensure.NotNull(exceptionHandler, "exceptionHandler");
            Ensure.NotNull(authenticationState, "authenticationState");
            Ensure.NotNull(logFactory, "logFactory");
            Ensure.NotNull(json, "json");
            this.configuration = configuration.Value;
            this.http = http;
            this.commandMapper = commandMapper;
            this.queryMapper = queryMapper;
            this.exceptionHandler = exceptionHandler;
            this.authenticationState = authenticationState;
            this.log = logFactory.Scope("ApiClient");
            this.json = json;

            http.BaseAddress = this.configuration.ApiUrl;
        }

        public async Task<bool> LoginAsync(string userName, string password, bool isPermanent)
        {
            string requestContent = json.Serialize(new LoginRequest()
            {
                UserName = userName,
                Password = password
            });

            HttpResponseMessage responseMessage = await http.PostAsync("/api/user/login", new StringContent(requestContent, Encoding.UTF8, "text/json"));
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string responseContent = await responseMessage.Content.ReadAsStringAsync();
                LoginResponse response = json.Deserialize<LoginResponse>(responseContent);
                Console.WriteLine(responseContent);
                Console.WriteLine(response.Token);

                if (!String.IsNullOrEmpty(response.Token))
                {
                    await authenticationState.SetTokenAsync(response.Token, isPermanent);
                    return true;
                }
            }

            return false;
        }

        public Task LogoutAsync() 
            => authenticationState.ClearTokenAsync();

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
            string url = queryMapper.FindUrlByType(type);
            if (url != null)
            {
                try
                {
                    HttpResponseMessage response = await http.PostAsync($"/api/query/{url}", new StringContent(payload, Encoding.UTF8, "text/json"));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        log.Debug($"Response: '{responseContent}'.");
                        return json.Deserialize<Response>(responseContent);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await authenticationState.ClearTokenAsync();
                        throw new UnauthorizedAccessException();
                    }
                    else if (response.StatusCode == HttpStatusCode.InternalServerError)
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
    }
}
