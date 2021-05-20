using Microsoft.AspNetCore.Components;
using Money.Events;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected bool IsUpdateable { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            EventHandlers.Add<PwaUpdateable>(this);
        }

        public void Dispose()
        {
            EventHandlers.Remove<PwaUpdateable>(this);
        }

        Task IEventHandler<PwaUpdateable>.HandleAsync(PwaUpdateable payload)
        {
            IsUpdateable = true;
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
}
