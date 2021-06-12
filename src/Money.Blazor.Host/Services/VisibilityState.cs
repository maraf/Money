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
    public class VisibilityState
    {
        private readonly ILog log;

        public event Action StatusChanged;

        public VisibilityState(VisibilityStateInterop interop, ILog<VisibilityState> log)
        {
            Ensure.NotNull(interop, "interop");
            Ensure.NotNull(log, "log");
            this.log = log;

            _ = interop.InitializeAsync(OnStatusChanged);
        }

        private void OnStatusChanged(bool isVisible)
        {
            if (IsVisible != isVisible)
            {
                log.Debug($"IsVisible changed {IsVisible} => '{isVisible}'.");

                IsVisible = isVisible;
                StatusChanged?.Invoke();
            }
        }

        public bool IsVisible { get; protected set; } = true;
    }
}
