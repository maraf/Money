using Microsoft.AspNetCore.Blazor;
using Money.Models.Api;
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
            http.BaseAddress = new Uri("http://localhost:63803");
            //http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZGVtbyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMjhmNGQxNzYtNjg5ZS00ZDRkLTlhMzgtYTg3MGQ5NzFhZDc5IiwiZXhwIjoxNTUyNzI2NDU2LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdCIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0In0.4tSJlngLynld3Ul_HuicpO4zUERjYZ4FFjTrJxfE8Po");
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
