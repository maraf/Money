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
        public event Action StatusChanged;

        public NetworkState(NetworkStateInterop interop)
        {
            Ensure.NotNull(interop, "interop");
            _ = interop.InitializeAsync(OnStatusChanged);
        }

        private void OnStatusChanged(bool isOnline)
        {
            if (IsOnline != isOnline)
            {
                Console.WriteLine($"NetworkState: IsOnline changed {IsOnline} => '{isOnline}'.");

                IsOnline = isOnline;
                StatusChanged?.Invoke();
            }
        }

        public bool IsOnline { get; protected set; } = true;
    }
}
