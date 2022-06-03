using Microsoft.AspNetCore.Components;
using Money.Events;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class PwaUpdate : IDisposable,
        IEventHandler<PwaUpdateable>
    {
        [Inject]
        protected IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected Navigator Navigator { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected bool IsUpdateable { get; set; }
        protected MarkupString ReleaseNotes { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            EventHandlers.Add<PwaUpdateable>(this);
        }

        public void Dispose()
        {
            EventHandlers.Remove<PwaUpdateable>(this);
        }

        async Task IEventHandler<PwaUpdateable>.HandleAsync(PwaUpdateable payload)
        {
            IsUpdateable = true;
            StateHasChanged();
        }
    }
}
