using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
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
    public class ApiHubService : IApiHubState
    {
        private readonly BrowserEventDispatcher events;
        private readonly BrowserExceptionHandler exceptions;
        private readonly Navigator navigator;
        private readonly ApiClientConfiguration apiConfiguration;
        private readonly TokenContainer token;
        private readonly ILog log;
        private HubConnection connection;

        public bool IsActive { get; private set; }
        public bool IsError { get; private set; }
        public event Action Changed;

        public ApiHubService(BrowserEventDispatcher events, BrowserExceptionHandler exceptions, Navigator navigator, IOptions<ApiClientConfiguration> apiConfiguration, TokenContainer token, ILogFactory logFactory)
        {
            Ensure.NotNull(events, "events");
            Ensure.NotNull(exceptions, "exceptions");
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(apiConfiguration, "apiConfiguration");
            Ensure.NotNull(token, "token");
            Ensure.NotNull(logFactory, "logFactory");
            this.events = events;
            this.exceptions = exceptions;
            this.navigator = navigator;
            this.apiConfiguration = apiConfiguration.Value;
            this.token = token;
            this.log = logFactory.Scope("ApiHub");
        }

        public async Task StartAsync()
        {
            await StopAsync();

            log.Debug($"Connecting with token '{token.Value}'.");

            string url = $"{apiConfiguration.ApiUrl}api?access_token={token.Value}";

            connection = new HubConnectionBuilder()
                .WithUrl(url, o => o.AccessTokenProvider = () => Task.FromResult(token.Value))
                .Build();

            connection.On<string>(ApiHubMethod.RaiseEvent, payload =>
            {
                log.Debug($"Event: {payload}");
                events.Raise(payload);
            });

            connection.On<string>(ApiHubMethod.RaiseException, payload =>
            {
                log.Debug($"Exception: {payload}");
                exceptions.Raise(payload);
            });

            connection.Closed += OnConnectionClosed;

            await connection.StartAsync();

            ChangeState(true);
        }

        private Task OnConnectionClosed(Exception e)
        {
            log.Debug("Connection closed.");

            bool isError = false;
            if (e != null)
            {
                log.Fatal($"Connection error {Environment.NewLine}{e}");
                isError = true;

                connection.Closed -= OnConnectionClosed;
                connection = null;
            }

            ChangeState(false, isError);
            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            if (connection != null)
            {
                log.Debug($"Disconnecting.");
                connection.Closed -= OnConnectionClosed;

                await connection.StopAsync();
                await connection.DisposeAsync();
                connection = null;
            }

            ChangeState(false);
        }

        private void ChangeState(bool isActive, bool isError = false)
        {
            IsActive = isActive;

            if (isError)
                IsError = true;

            Changed?.Invoke();
        }
    }
}
