using Microsoft.AspNetCore.Components;
using Money.Components.Bootstrap;
using Money.Services;
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
        public IApiHubState State { get; set; }

        [Inject]
        public Navigator Navigator { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

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
            if (LastException != null)
                Dialog.Show();
            else
                Dialog.Hide();
        }

        protected Task ReloadAsync()
            => Navigator.ReloadAsync();

        public void Dispose()
            => State.Changed -= OnStateChanged;
    }
}
