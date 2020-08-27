using Microsoft.AspNetCore.Components;
using Money.Services;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class PwaInstall : ComponentBase, IDisposable
    {
        [Inject]
        internal PwaInstallInterop Interop { get; set; }

        [Inject]
        internal ILog<PwaInstall> Log { get; set; }

        [Inject]
        internal Navigator Navigator { get; set; }

        protected ElementReference Button { get; set; }
        protected bool IsInstallable { get; set; }
        protected bool IsUpdateable { get; set; }

        protected override void OnInitialized()
        {
            Log.Debug("OnInitialized");

            base.OnInitialized();
            Interop.Initialize(this);
        }

        //protected async override Task OnAfterRenderAsync(bool firstRender)
        //{
        //    await base.OnAfterRenderAsync(firstRender);

        //    if (IsInstallable)
        //        await Tooltip.InitializeAsync(Button);
        //}

        public void MakeInstallable()
        {
            Log.Debug("Installable=True");

            IsInstallable = true;
            StateHasChanged();
        }

        public void MakeUpdateable()
        {
            Log.Debug("Updateable=True");

            IsUpdateable = true;
            StateHasChanged();
        }

        protected async Task InstallAsync()
        {
            await Interop.InstallAsync();
            IsInstallable = false;
        }

        protected async Task UpdateAsync() 
            => await Navigator.ReloadAsync();

        public void Dispose()
        {
            Interop.Remove(this);
        }
    }
}
