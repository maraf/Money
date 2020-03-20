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

        public SignalRListener(ApiHubService apiHub)
        {
            Ensure.NotNull(apiHub, "apiHub");
            this.apiHub = apiHub;
        }

        Task IEventHandler<UserSignedIn>.HandleAsync(UserSignedIn payload) 
            => apiHub.StartAsync();

        Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload) 
            => apiHub.StopAsync();
    }
}
