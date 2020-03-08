using Microsoft.Extensions.Options;
using Money.Events;
using Neptuo;
using Neptuo.Events.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class SignalRListener : IEventHandler<UserSignedIn>, IEventHandler<UserSignedOut>
    {
        private readonly ApiHubService apiHub;
        private readonly ApiClientConfiguration apiConfiguration;
        private readonly TokenContainer token;

        public SignalRListener(ApiHubService apiHub, IOptions<ApiClientConfiguration> apiConfiguration, TokenContainer token)
        {
            Ensure.NotNull(apiHub, "apiHub");
            Ensure.NotNull(apiConfiguration, "apiConfiguration");
            Ensure.NotNull(token, "token");
            this.apiHub = apiHub;
            this.apiConfiguration = apiConfiguration.Value;
            this.token = token;
        }

        Task IEventHandler<UserSignedIn>.HandleAsync(UserSignedIn payload) 
            => apiHub.StartAsync(apiConfiguration.ApiUrl.ToString() + "api", token.Value);

        Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload) 
            => apiHub.StopAsync();
    }
}
