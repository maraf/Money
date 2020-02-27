using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Neptuo;
using Neptuo.Events;
using Neptuo.Exceptions;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class ApiHubService
    {
        private readonly BrowserEventDispatcher events;
        private readonly BrowserExceptionHandler exceptions;
        private readonly Navigator navigator;
        private readonly ILog log;
        private HubConnection connection;

        public bool IsStarted { get; private set; }

        public ApiHubService(BrowserEventDispatcher events, BrowserExceptionHandler exceptions, Navigator navigator, ILogFactory logFactory)
        {
            Ensure.NotNull(events, "events");
            Ensure.NotNull(exceptions, "exceptions");
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(logFactory, "logFactory");
            this.events = events;
            this.exceptions = exceptions;
            this.navigator = navigator;
            this.log = logFactory.Scope("ApiHub");
        }

        public async Task StartAsync(string url, string token)
        {
            await StopAsync();

            log.Debug($"Connecting with token '{token}'.");

            url = $"{url}?access_token={token}";

            connection = new HubConnectionBuilder()
                .WithUrl(url, o => o.AccessTokenProvider = () => Task.FromResult(token))
                .Build();

            connection.On<string>("RaiseEvent", payload =>
            {
                log.Debug($"Event: {payload}");
                events.Raise(payload);
            });

            connection.On<string>("RaiseException", payload =>
            {
                log.Debug($"Exception: {payload}");
                exceptions.Raise(payload);
            });

            connection.Closed += async e =>
            {
                log.Debug("Connection closed.");

                if (e != null)
                    log.Fatal($"Connection error {Environment.NewLine}{e.ToString()}");

#if !DEBUG
                await navigator.AlertAsync("Underlaying connection to the server has closed. Reloading the page...");
#endif

                await Task.Delay(2000);
                await navigator.ReloadAsync();
            };

            await connection.StartAsync();

            IsStarted = true;
        }

        public async Task StopAsync()
        {
            if (connection != null)
            {
                log.Debug($"Disconnecting.");
                await connection.StopAsync();
                await connection.DisposeAsync();
                connection = null;
            }
        }
    }
}
