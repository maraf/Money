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
        private readonly Interop interop;
        private readonly ApiClientConfiguration apiConfiguration;
        private readonly TokenContainer token;

        public SignalRListener(Interop interop, IOptions<ApiClientConfiguration> apiConfiguration, TokenContainer token)
        {
            Ensure.NotNull(interop, "interop");
            Ensure.NotNull(apiConfiguration, "apiConfiguration");
            Ensure.NotNull(token, "token");
            this.interop = interop;
            this.apiConfiguration = apiConfiguration.Value;
            this.token = token;
        }

        Task IEventHandler<UserSignedIn>.HandleAsync(UserSignedIn payload)
        {
            interop.StartSignalR(apiConfiguration.ApiUrl.ToString() + "api", token.Value);
            return Task.CompletedTask;
        }

        Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
        {
            interop.StopSignalR();
            return Task.CompletedTask;
        }
    }
}
