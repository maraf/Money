using Microsoft.AspNetCore.Components;
using Money.Components;
using Money.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    public partial class App
    {
        [Inject]
        public Interop Interop { get; set; }

        [Inject]
        public PullToRefreshInterop PullToRefresh { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            await Interop.AnimateSplashAsync();
            await PullToRefresh.InitializeAsync();
        }
    }
}
