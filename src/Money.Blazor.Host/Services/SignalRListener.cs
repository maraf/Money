using Money.Events;
using Neptuo;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class SignalRListener : IEventHandler<UserSignedIn>, IEventHandler<UserSignedOut>, IEventHandler<VisibilityChanged>
    {
        private readonly ApiHubService apiHub;
        private readonly ILog log;

        public SignalRListener(ApiHubService apiHub, ILogFactory logFactory)
        {
            Ensure.NotNull(apiHub, "apiHub");
            Ensure.NotNull(logFactory, "logFactory");
            this.apiHub = apiHub;
            this.log = logFactory.Scope("SignalRListener");
        }

        Task IEventHandler<UserSignedIn>.HandleAsync(UserSignedIn payload) 
            => apiHub.StartAsync();

        Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload) 
            => apiHub.StopAsync();

        public Task HandleAsync(VisibilityChanged payload)
        {
            log.Debug($"Got event '{nameof(VisibilityChanged)}' is value '{payload.IsVisible}'.");

            if (payload.IsVisible && apiHub.Status != ApiHubStatus.Connected)
            {
                log.Debug("Starting connection.");
                return apiHub.StartAsync();
            }

            return Task.CompletedTask;
        }
    }
}
