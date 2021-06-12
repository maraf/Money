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
    public class VisibilityState
    {
        private readonly IEventDispatcher events;
        private readonly ILog log;

        public event Action StatusChanged;

        public VisibilityState(VisibilityStateInterop interop, IEventDispatcher events, ILog<VisibilityState> log)
        {
            Ensure.NotNull(interop, "interop");
            Ensure.NotNull(events, "events");
            Ensure.NotNull(log, "log");
            this.events = events;
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

                _ = events.PublishAsync(new VisibilityChanged(IsVisible));
            }
        }

        public bool IsVisible { get; protected set; } = true;
    }
}
