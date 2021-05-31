using Microsoft.AspNetCore.Components;
using Money.Components.Bootstrap;
using Money.Services;
using Neptuo.Exceptions.Handlers;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class ApiHubConnectionChecker : IDisposable
    {
        [Inject]
        public ServerConnectionState ServerConnection { get; set; }

        [Inject]
        public ApiHubService ApiHub { get; set; }

        [Inject]
        public Navigator Navigator { get; set; }

        [Inject]
        public ILog<ApiHubConnectionChecker> Log { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Log.Debug("Bind on changed.");
            ServerConnection.Changed += OnStateChanged;
            ApiHub.Changed += OnApiHubStateChanged;
        }

        private void OnApiHubStateChanged(ApiHubStatus status, Exception e)
        {
            Log.Debug("OnApiHubStateChanged, rerendering.");
            StateHasChanged();
        }

        private void OnStateChanged(bool isAvailable)
        {
            Log.Debug("OnStateChanged, rerendering.");
            StateHasChanged();
        }

        protected Task ReloadAsync()
            => Navigator.ReloadAsync();

        public void Dispose()
        {
            ServerConnection.Changed -= OnStateChanged;
            ApiHub.Changed -= OnApiHubStateChanged;
        }

        protected Task ReconnectAsyc()
            => ApiHub.StartAsync();
    }
}
