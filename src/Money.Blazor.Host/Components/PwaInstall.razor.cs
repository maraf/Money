using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Events;
using Money.Queries;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class PwaInstall : ComponentBase, IDisposable,
        IEventHandler<PwaInstallable>,
        IEventHandler<PwaUpdateable>
    {
        [Inject]
        internal PwaInstallInterop Interop { get; set; }

        [Inject]
        internal ILog<PwaInstall> Log { get; set; }

        [Inject]
        internal Navigator Navigator { get; set; }

        [Inject]
        protected IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        protected ElementReference Button { get; set; }
        protected bool IsInstallable { get; set; }
        protected bool IsUpdateable { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Log.Debug("OnInitialized");

            await base.OnInitializedAsync();

            EventHandlers
                .Add<PwaInstallable>(this)
                .Add<PwaUpdateable>(this);

            var status = await Queries.QueryAsync(new GetPwaStatus());

            IsInstallable = status.IsInstallable;
            IsUpdateable = status.IsUpdateable;
        }

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
            await Commands.HandleAsync(new InstallPwa());
        }

        protected async Task UpdateAsync()
        {
            await Commands.HandleAsync(new UpdatePwa());
        }

        Task IEventHandler<PwaInstallable>.HandleAsync(PwaInstallable payload)
        {
            MakeInstallable();
            return Task.CompletedTask;
        }

        Task IEventHandler<PwaUpdateable>.HandleAsync(PwaUpdateable payload)
        {
            MakeUpdateable();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            EventHandlers
                .Remove<PwaInstallable>(this)
                .Remove<PwaUpdateable>(this);
        }
    }
}
