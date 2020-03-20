using Microsoft.Extensions.Options;
using Money.Models.Api;
using Money.Users.Models;
using Neptuo;
using Neptuo.Exceptions.Handlers;
using Neptuo.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class ApiClient
    {
        private readonly ApiVersionChecker versionChecker;
        private readonly HttpClient http;
        private readonly CommandMapper commandMapper;
        private readonly QueryMapper queryMapper;
        private readonly IExceptionHandler exceptionHandler;
        private readonly ApiAuthenticationStateProvider authenticationState;
        private readonly ILog log;
        private readonly Json json;

        public ApiClient(IOptions<ApiClientConfiguration> configuration, ApiVersionChecker versionChecker, HttpClient http, CommandMapper commandMapper, QueryMapper queryMapper, IExceptionHandler exceptionHandler, ApiAuthenticationStateProvider authenticationState, ILogFactory logFactory, Json json)
        {
            Ensure.NotNull(configuration, "configuration");
            Ensure.NotNull(versionChecker, "versionChecker");
            Ensure.NotNull(http, "http");
            Ensure.NotNull(commandMapper, "commandMapper");
            Ensure.NotNull(queryMapper, "queryMapper");
            Ensure.NotNull(exceptionHandler, "exceptionHandler");
            Ensure.NotNull(authenticationState, "authenticationState");
            Ensure.NotNull(logFactory, "logFactory");
            Ensure.NotNull(json, "json");
            this.versionChecker = versionChecker;
            this.http = http;
            this.commandMapper = commandMapper;
            this.queryMapper = queryMapper;
            this.exceptionHandler = exceptionHandler;
            this.authenticationState = authenticationState;
            this.log = logFactory.Scope("ApiClient");
            this.json = json;

            this.http.BaseAddress = configuration.Value.ApiUrl;
        }

        private HttpContent CreateJsonContent<T>(T request)
        {
            Ensure.NotNull(request, "request");
            string requestContent = json.Serialize(request);
            return CreateStringContent(requestContent);
        }

        private static StringContent CreateStringContent(string requestContent)
            => new StringContent(requestContent, Encoding.UTF8, "text/json");

        private async Task EnsureSuccessResponseAsync(HttpResponseMessage responseMessage)
        {
            log.Debug($"Api headers: '{responseMessage.Headers}'.");

            if (responseMessage.Headers.TryGetValues(VersionHeader.Name, out var versions))
            {
                var rawVersion = versions.FirstOrDefault();

                log.Debug($"ApiVersion is '{rawVersion}'.");

                if (!String.IsNullOrEmpty(rawVersion) && Version.TryParse(rawVersion, out var version))
                    versionChecker.Ensure(version);
            }

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
            await EnsureSuccessResponseAsync(responseMessage);

            string responseContent = await responseMessage.Content.ReadAsStringAsync();
            log.Debug($"Response: '{responseContent}'.");

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

            try
            {
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
            catch (Exception e)
            {
                throw ProcessHttpException(e);
            }
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

            try
            {
                HttpResponseMessage responseMessage = await http.PostAsync("/api/user/register", requestContent);
                RegisterResponse response = await ReadJsonResponseAsync<RegisterResponse>(responseMessage);
                return response;
            }
            catch (Exception e)
            {
                throw ProcessHttpException(e);
            }
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
                throw ProcessHttpException(e);
            }
        }

        public async Task CommandAsync(Type type, string payload)
        {
            try
            {
                HttpResponseMessage responseMessage = await PostToUniformApiAsync(commandMapper, "/api/command", type, payload);
                await EnsureSuccessResponseAsync(responseMessage);
            }
            catch (Exception e)
            {
                throw ProcessHttpException(e);
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

        private Exception ProcessHttpException(Exception e)
        {
            log.Debug($"Exception while invoking HTTP request, type: '{e.GetType().FullName}', message: '{e.Message}'.");

            if (e.Message == "TypeError: Failed to fetch" || e is HttpRequestException)
                e = new ServerNotRespondingException(e);

            exceptionHandler.Handle(e);
            return e;
        }
    }
}
