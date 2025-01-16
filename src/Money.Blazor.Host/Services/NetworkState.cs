using Neptuo;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class NetworkState
    {
        private readonly ILog log;

        public event Action StatusChanged;

        public NetworkState(NetworkStateInterop interop, ILog<NetworkState> log)
        {
            Ensure.NotNull(interop, "interop");
            Ensure.NotNull(log, "log");
            this.log = log;

            _ = interop.InitializeAsync(OnStatusChanged);
        }

        private void OnStatusChanged(bool isOnline)
        {
            if (IsOnline != isOnline)
            {
                log.Debug($"IsVisible IsOnline changed {IsOnline} => '{isOnline}'.");

                IsOnline = isOnline;
                StatusChanged?.Invoke();
            }
        }

        public bool IsOnline { get; protected set; } = true;
    }
}
