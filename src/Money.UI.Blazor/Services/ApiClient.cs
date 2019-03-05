using Microsoft.AspNetCore.Blazor;
using Money.Models.Api;
using Money.Users.Models;
using Neptuo;
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
        private const string rootUrl = "http://localhost:63803";
        private static string token;

        private readonly HttpClient http;
        private readonly CommandMapper commandMapper;
        private readonly QueryMapper queryMapper;
        private readonly IExceptionHandler exceptionHandler;

        public ApiClient(HttpClient http, CommandMapper commandMapper, QueryMapper queryMapper, IExceptionHandler exceptionHandler)
        {
            Ensure.NotNull(http, "http");
            Ensure.NotNull(commandMapper, "commandMapper");
            Ensure.NotNull(queryMapper, "queryMapper");
            Ensure.NotNull(exceptionHandler, "exceptionHandler");
            this.http = http;
            this.commandMapper = commandMapper;
            this.queryMapper = queryMapper;
            this.exceptionHandler = exceptionHandler;
            http.BaseAddress = new Uri(rootUrl);

            EnsureAuthorization();
        }

        private void ClearAuthorization()
        {
            token = null;
            http.DefaultRequestHeaders.Authorization = null;
            Interop.StopSignalR();
        }

        private void EnsureAuthorization()
        {
            if (token != null && http.DefaultRequestHeaders.Authorization?.Parameter != token)
            {
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Interop.StartSignalR(rootUrl + "/api", token);
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
                token = response.Token;
                EnsureAuthorization();
                return true;
            }

            return false;
        }

        private Request CreateRequest(Type type, string payload)
            => new Request() { Type = type.AssemblyQualifiedName, Payload = payload };

        public async Task<Response> QueryAsync(Type type, string payload)
        {
            string url = queryMapper.FindUrlByType(type);
            if (url != null)
            {
                HttpResponseMessage response = await http.PostAsync($"/api/query/{url}", new StringContent(payload, Encoding.UTF8, "text/json"));
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ClearAuthorization();
                    UnauthorizedAccessException exception = new UnauthorizedAccessException();
                    exceptionHandler.Handle(exception);
                    throw exception;
                }

                string responseContent = await response.Content.ReadAsStringAsync();
                return SimpleJson.SimpleJson.DeserializeObject<JsResponse>(responseContent);
            }
            else
                return await http.PostJsonAsync<Response>($"/api/query", CreateRequest(type, payload));
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
