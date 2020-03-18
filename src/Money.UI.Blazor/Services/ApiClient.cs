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

        private HttpContent CreateJsonContent<T>(T request)
        {
            Ensure.NotNull(request, "request");
            string requestContent = json.Serialize(request);
            return CreateStringContent(requestContent);
        }

        private static StringContent CreateStringContent(string requestContent)
            => new StringContent(requestContent, Encoding.UTF8, "text/json");

        private async Task EnsureStatusCodeAsync(HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                return;
            }
            else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                await authenticationState.ClearTokenAsync();
                throw new UnauthorizedAccessException();
            }
            else if (responseMessage.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new InternalServerException();
            }
            else
            {
                throw Ensure.Exception.InvalidOperation($"Generic HTTP error: {responseMessage.StatusCode}.");
            }
        }

        private async Task<T> ReadJsonResponseAsync<T>(HttpResponseMessage responseMessage)
        {
            string responseContent = await responseMessage.Content.ReadAsStringAsync();
            log.Debug($"Response: '{responseContent}'.");

            await EnsureStatusCodeAsync(responseMessage);

            T response = json.Deserialize<T>(responseContent);
            return response;
        }

        public async Task<bool> LoginAsync(string userName, string password, bool isPermanent)
        {
            HttpContent requestContent = CreateJsonContent(new LoginRequest()
            {
                UserName = userName,
                Password = password
            });

            HttpResponseMessage responseMessage = await http.PostAsync("/api/user/login", requestContent);
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                LoginResponse response = await ReadJsonResponseAsync<LoginResponse>(responseMessage);
                log.Debug($"Login success, token '{response.Token}'.");

                if (!String.IsNullOrEmpty(response.Token))
                {
                    await authenticationState.SetTokenAsync(response.Token, isPermanent);
                    return true;
                }
            }

            log.Debug($"Login failed, status code '{responseMessage.StatusCode}'.");
            return false;
        }

        public Task LogoutAsync()
            => authenticationState.ClearTokenAsync();

        public async Task<RegisterResponse> RegisterAsync(string userName, string password)
        {
            HttpContent requestContent = CreateJsonContent(new RegisterRequest()
            {
                UserName = userName,
                Password = password
            });

            HttpResponseMessage responseMessage = await http.PostAsync("/api/user/register", requestContent);
            RegisterResponse response = await ReadJsonResponseAsync<RegisterResponse>(responseMessage);
            return response;
        }

        private Request CreateRequest(Type type, string payload)
            => new Request() { Type = type.AssemblyQualifiedName, Payload = payload };

        public async Task<Response> QueryAsync(Type type, string payload)
        {
            try
            {
                HttpResponseMessage responseMessage = await PostToUniformApiAsync(queryMapper, "/api/query", type, payload);
                return await ReadJsonResponseAsync<Response>(responseMessage);
            }
            catch (Exception e)
            {
                if (e is HttpRequestException)
                    e = new ServerNotRespondingException(e);

                exceptionHandler.Handle(e);
                throw;
            }
        }

        public async Task CommandAsync(Type type, string payload)
        {
            try
            {
                HttpResponseMessage responseMessage = await PostToUniformApiAsync(commandMapper, "/api/command", type, payload);
                await EnsureStatusCodeAsync(responseMessage);
            }
            catch (Exception e)
            {
                if (e is HttpRequestException)
                    e = new ServerNotRespondingException(e);

                exceptionHandler.Handle(e);
                throw;
            }
        }

        private Task<HttpResponseMessage> PostToUniformApiAsync(TypeMapper mapper, string baseUrl, Type type, string payload)
        {
            string url = mapper.FindUrlByType(type);
            if (url != null)
                return http.PostAsync($"{baseUrl}/{url}", CreateStringContent(payload));
            else
                return http.PostAsync(baseUrl, CreateJsonContent(CreateRequest(type, payload)));
        }
    }
}
