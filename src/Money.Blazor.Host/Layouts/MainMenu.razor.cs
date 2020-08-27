using Microsoft.AspNetCore.Components;
using Money.Events;
using Money.Services;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public class MainMenuBase : ComponentBase, IDisposable, 
        IEventHandler<UserSignedOut>
    {
        protected const string MenuLeftMarginCssClass = "ml-2 ml-lg-0";

        [Inject]
        internal Navigator Navigator {get;set;}

        [Inject]
        internal ILog<MainMenuBase> Log { get; set; }

        [Inject]
        internal IEventHandlerCollection EventHandlers { get; set; }

        protected bool IsMainMenuVisible { get; set; } = false;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Navigator.LocationChanged += OnLocationChanged;
            EventHandlers.Add<UserSignedOut>(this);
        }

        public void Dispose()
        {
            Navigator.LocationChanged -= OnLocationChanged;
            EventHandlers.Remove<UserSignedOut>(this);
        }

        private void UpdateMainMenuVisible(bool isVisible)
        {
            if (IsMainMenuVisible != isVisible)
            {
                Log.Debug($"IsMainMenuVisible: Previous='{IsMainMenuVisible}', New='{isVisible}'.");
                IsMainMenuVisible = isVisible;
                StateHasChanged();
            }
        }

        Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
        {
            UpdateMainMenuVisible(false);
            return Task.CompletedTask;
        }

        private void OnLocationChanged(string url)
            => UpdateMainMenuVisible(false);

        protected void OnMainMenuToggleClick()
            => UpdateMainMenuVisible(!IsMainMenuVisible);
    }
}
