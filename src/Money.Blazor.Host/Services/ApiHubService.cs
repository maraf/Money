using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Money.Hubs;
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
        private readonly ApiConfiguration apiConfiguration;
        private readonly TokenContainer token;
        private readonly ILog log;
        private readonly ILog eventLog;
        private readonly ILog exceptionLog;
        private HubConnection connection;

        public ApiHubStatus Status { get; private set; }
        public event Action<ApiHubStatus, Exception> Changed;

        public ApiHubService(BrowserEventDispatcher events, BrowserExceptionHandler exceptions, IOptions<ApiConfiguration> apiConfiguration, TokenContainer token, ILogFactory logFactory)
        {
            Ensure.NotNull(events, "events");
            Ensure.NotNull(exceptions, "exceptions");
            Ensure.NotNull(apiConfiguration, "apiConfiguration");
            Ensure.NotNull(token, "token");
            Ensure.NotNull(logFactory, "logFactory");
            this.events = events;
            this.exceptions = exceptions;
            this.apiConfiguration = apiConfiguration.Value;
            this.token = token;
            this.log = logFactory.Scope("ApiHub");
            this.eventLog = log.Factory.Scope("Events");
            this.exceptionLog = log.Factory.Scope("Exceptions");
        }

        public async Task StartAsync()
        {
            log.Debug($"Starting.");

            await StopAsync();

            ChangeStatus(ApiHubStatus.Connecting);

            try
            {
                log.Debug($"Connecting with token '{token.Value}'.");

                string url = $"{apiConfiguration.ApiUrl}api";

                connection = new HubConnectionBuilder()
                    .WithUrl(url, o => o.AccessTokenProvider = () => Task.FromResult(token.Value))
                    .WithAutomaticReconnect()
                    .Build();

                connection.On<string>(ApiHubMethod.RaiseEvent, RaiseEvent);
                connection.On<string>(ApiHubMethod.RaiseException, RaiseException);
                connection.Reconnecting += OnConnectionReconnectingAsync;
                connection.Reconnected += OnConnectionReconnectedAsync;
                connection.Closed += OnConnectionClosedAsync;

                await connection.StartAsync();

                ChangeStatus(ApiHubStatus.Connected);
            }
            catch (Exception e)
            {
                ChangeStatus(ApiHubStatus.Disconnected);
            }
        }

        private void RaiseEvent(string payload)
        {
            eventLog.Debug(payload);
            events.Raise(payload);
        }

        private void RaiseException(string payload)
        {
            exceptionLog.Debug(payload);
            exceptions.Raise(payload);
        }

        private Task OnConnectionReconnectingAsync(Exception e)
        {
            log.Debug($"Reconnecting {Environment.NewLine}{e.ToString()}");
            ChangeStatus(ApiHubStatus.Connecting, e);
            return Task.CompletedTask;
        }

        private Task OnConnectionReconnectedAsync(string arg)
        {
            log.Debug($"Reconnected: {arg}");
            ChangeStatus(ApiHubStatus.Connected);
            return Task.CompletedTask;
        }

        private Task OnConnectionClosedAsync(Exception e)
        {
            log.Debug("Connection closed.");

            if (e != null)
            {
                log.Fatal($"Connection error {Environment.NewLine}{e.ToString()}");

                connection.Closed -= OnConnectionClosedAsync;
                connection = null;
            }

            ChangeStatus(ApiHubStatus.Disconnected, e);
            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            var connection = this.connection;
            if (connection != null)
            {
                this.connection = null;

                log.Debug($"Disconnecting.");

                await connection.StopAsync();
                await connection.DisposeAsync();
            }

            ChangeStatus(ApiHubStatus.Disconnected);
        }

        private void ChangeStatus(ApiHubStatus status, Exception e = null)
        {
            if (Status != status || e != null)
            {
                log.Debug($"Change status from '{Status}' to '{status}'.");

                Status = status;
                Changed?.Invoke(status, e);
            }
        }
    }
}
