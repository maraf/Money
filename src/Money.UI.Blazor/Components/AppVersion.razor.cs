using Microsoft.AspNetCore.Components;
using Money.Events;
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
    public partial class AppVersion : IDisposable,
        IEventHandler<ApiVersionChanged>
    {
        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        protected System.Version Client { get; set; }
        protected System.Version Api { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            Client = typeof(AppVersion).Assembly.GetName().Version;
            EventHandlers.Add<ApiVersionChanged>(this);
        }

        public void Dispose()
        {
            EventHandlers.Remove<ApiVersionChanged>(this);
        }

        Task IEventHandler<ApiVersionChanged>.HandleAsync(ApiVersionChanged payload)
        {
            Api = payload.ApiVersion;
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
}
