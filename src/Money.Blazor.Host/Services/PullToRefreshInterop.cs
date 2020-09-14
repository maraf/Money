using Microsoft.JSInterop;
using Money.Events;
using Neptuo;
using Neptuo.Events;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class PullToRefreshInterop
    {
        private readonly IJSRuntime jsRuntime;
        private readonly IEventDispatcher events;
        private readonly Navigator navigator;
        private readonly ILog log;

        public PullToRefreshInterop(IJSRuntime jsRuntime, IEventDispatcher events, Navigator navigator, ILogFactory logFactory)
        {
            Ensure.NotNull(jsRuntime, "jsRuntime");
            Ensure.NotNull(events, "events");
            Ensure.NotNull(navigator, "navigator");
            this.jsRuntime = jsRuntime;
            this.events = events;
            this.navigator = navigator;
            this.log = logFactory.Scope("PullToRefresh");
        }

        public async Task InitializeAsync()
        {
            log.Debug("Registering handler.");
            await jsRuntime.InvokeVoidAsync("PullToRefresh.Initialize", DotNetObjectReference.Create(this));
        }

        [JSInvokable("PullToRefresh.Pulled")]
        public void OnPulled() 
            => _ = RaiseEventAsync();

        private async Task RaiseEventAsync()
        {
            log.Debug("Raising event.");

            var e = new PulledToRefresh();
            await events.PublishAsync(e);

            log.Debug($"Event handled result '{e.IsHandled}'.");
            if (!e.IsHandled)
                await navigator.ReloadAsync();
        }
    }
}
