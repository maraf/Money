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

        public ApiClient(HttpClient http)
        {
            Ensure.NotNull(http, "http");
            this.http = http;
        }

        public Task<string> GetUserNameAsync() => http.GetStringAsync("/api/username");

        public async Task<Response> QueryAsync(Request request)
        {
            return await http.PostJsonAsync<Response>("/api/query", request);

            //var requestJson = JsonUtil.Serialize(request);
            //var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/query")
            //{
            //    Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            //};

            //var responseMessage = await http.SendAsync(requestMessage);
            //var responseJson = await responseMessage.Content.ReadAsStringAsync();

            //Console.WriteLine($"API Response: {responseJson}");

            //var response = JsonUtil.Deserialize<Response>(responseJson);
            //return response;
        }
    }
}
