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
    }
}
