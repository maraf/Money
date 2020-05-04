using Neptuo;
using Neptuo.Exceptions.Handlers;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class ServerConnectionState : System.IDisposable
    {
        private readonly NetworkState network;
        private readonly IApiHubState hub;
        private readonly ILog log;

        private bool isHubConnected = true;
        private bool isServerException;

        public bool IsAvailable => network.IsOnline && isHubConnected && !isServerException;
        public Action<bool> Changed;

        public ServerConnectionState(NetworkState network, IApiHubState hub, ExceptionHandlerBuilder exceptionHandlers, ILogFactory logFactory)
        {
            Ensure.NotNull(network, "network");
            Ensure.NotNull(hub, "hub");
            Ensure.NotNull(exceptionHandlers, "exceptionHandlers");
            Ensure.NotNull(logFactory, "logFactory");
            this.network = network;
            this.hub = hub;
            this.log = logFactory.Scope("ServerConnectionState");

            network.StatusChanged += OnNetworkChanged;
            hub.Changed += OnServerStateChanged;
            exceptionHandlers.Handler<ServerNotRespondingException>(OnServerNotRespondingException);
        }

        private void OnServerNotRespondingException(ServerNotRespondingException e)
        {
            log.Debug($"Catch '{nameof(ServerNotRespondingException)}'.");
            isServerException = true;
            RaiseChanged();
        }

        public void Dispose()
        {
            network.StatusChanged -= OnNetworkChanged;
            hub.Changed -= OnServerStateChanged;
        }

        private void OnNetworkChanged() =>
            RaiseChanged();

        private void RaiseChanged() 
            => Changed?.Invoke(IsAvailable);

        private void OnServerStateChanged(ApiHubStatus status, Exception e)
        {
            log.Debug($"ApiHub update '{status}', '{e?.GetType()?.Name}'.");
            if (status == ApiHubStatus.Disconnected && e != null)
            {
                isHubConnected = false;
                RaiseChanged();
            }
            else if (status == ApiHubStatus.Connected)
            {
                isServerException = false;
                isHubConnected = true;
                RaiseChanged();
            }
        }
    }
}
