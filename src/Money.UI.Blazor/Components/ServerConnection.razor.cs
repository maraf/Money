using Microsoft.AspNetCore.Components;
using Money.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class ServerConnection : IDisposable
    {
        [Inject]
        internal ServerConnectionState State { get; set; }

        [Parameter]
        public RenderFragment Online { get; set; }

        [Parameter]
        public RenderFragment Offline { get; set; }

        protected bool IsNetworkProblem { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            State.Changed += OnServerConnectionStateChanged;
        }

        public void Dispose() 
            => State.Changed -= OnServerConnectionStateChanged;

        private void OnServerConnectionStateChanged(bool isAvailable) 
            => StateHasChanged();
    }
}
