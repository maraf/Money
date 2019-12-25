using Microsoft.AspNetCore.Components;
using Money.Services;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class Network : System.IDisposable
    {
        [Inject]
        internal NetworkState State { get; set; }

        [Parameter]
        public RenderFragment Online { get; set; }

        [Parameter]
        public RenderFragment Offline { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            State.StatusChanged += StateHasChanged;
        }

        public void Dispose()
        {
            State.StatusChanged -= StateHasChanged;
        }
    }
}
