using Microsoft.AspNetCore.Components;
using Money.Services;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public class MainMenuBase : ComponentBase, IDisposable
    {
        protected const string MenuLeftMarginCssClass = "ml-2 ml-lg-0";

        [Inject]
        internal Navigator Navigator {get;set;}

        [Inject]
        internal ILog<MainMenuBase> Log { get; set; }

        protected bool IsMainMenuVisible { get; set; } = false;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Navigator.LocationChanged += OnLocationChanged;
        }

        public void Dispose()
        {
            Navigator.LocationChanged -= OnLocationChanged;
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

        private void OnLocationChanged(string url)
            => UpdateMainMenuVisible(false);

        protected void OnMainMenuToggleClick()
            => UpdateMainMenuVisible(!IsMainMenuVisible);
    }
}
