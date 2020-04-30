using Microsoft.AspNetCore.Components;
using Money.Components.Bootstrap;
using Money.Services;
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
        public ApiHubService State { get; set; }

        [Inject]
        public Navigator Navigator { get; set; }

        [Inject]
        public ILog<ApiHubConnectionChecker> Log { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected bool IsChangeIgnored { get; set; }
        protected Modal Dialog { get; set; }
        protected Exception LastException { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            State.Changed += OnStateChanged;
        }

        private void OnStateChanged(ApiHubStatus status, Exception e)
        {
            LastException = e;
            if (status == ApiHubStatus.Disconnected && LastException != null)
                Dialog.Show();
            else if (status == ApiHubStatus.Connected)
                Dialog.Hide();

            Log.Debug($"Status changed '{status}' ('{State.Status}') => SHC.");
            StateHasChanged();
        }

        protected Task ReloadAsync()
            => Navigator.ReloadAsync();

        public void Dispose()
            => State.Changed -= OnStateChanged;

        protected Task ReconnectAsyc() 
            => State.StartAsync();
    }
}
