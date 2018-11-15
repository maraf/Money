using Microsoft.AspNetCore.Blazor;
using Money.Models.Api;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class ApiClient
    {
        private readonly HttpClient http;
        private readonly QueryMapper queryMapper;

        public ApiClient(HttpClient http, QueryMapper queryMapper)
        {
            Ensure.NotNull(http, "http");
            Ensure.NotNull(queryMapper, "queryMapper");
            this.http = http;
            this.queryMapper = queryMapper;
        }

        public Task<string> GetUserNameAsync()
            => http.GetStringAsync("/api/username");

        public Task<Response> QueryAsync(Type type, string payload)
        {
            string url = queryMapper.FindUrlByType(type);
            if (url != null)
                return http.PostJsonAsync<Response>($"/api/query/{url}", payload);
            else
                return http.PostJsonAsync<Response>($"/api/query", new Request() { Type = type.AssemblyQualifiedName, Payload = payload });
        }

        public Task CommandAsync(Request request)
            => http.PostJsonAsync("/api/command", request);
    }
}
