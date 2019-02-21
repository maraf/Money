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
        private readonly CommandMapper commandMapper;
        private readonly QueryMapper queryMapper;

        public ApiClient(HttpClient http, CommandMapper commandMapper, QueryMapper queryMapper)
        {
            Ensure.NotNull(http, "http");
            Ensure.NotNull(commandMapper, "commandMapper");
            Ensure.NotNull(queryMapper, "queryMapper");
            this.http = http;
            this.commandMapper = commandMapper;
            this.queryMapper = queryMapper;
        }

        public Task<string> GetUserNameAsync()
            => http.GetStringAsync("/api/username");

        public Task ChangeEmail(string email)
            => http.PostJsonAsync("/api/user/changeemail", email);

        public Task ChangePassword(string currentPassword, string newPassword)
            => http.PostJsonAsync("/api/user/changeemail", new { Current = currentPassword, New = newPassword });

        private Request CreateRequest(Type type, string payload)
            => new Request() { Type = type.AssemblyQualifiedName, Payload = payload };

        public Task<Response> QueryAsync(Type type, string payload)
        {
            string url = queryMapper.FindUrlByType(type);
            if (url != null)
                return http.PostJsonAsync<Response>($"/api/query/{url}", payload);
            else
                return http.PostJsonAsync<Response>($"/api/query", CreateRequest(type, payload));
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
