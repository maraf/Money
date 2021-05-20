using Microsoft.JSInterop;
using Money.Commands;
using Money.Events;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Commands.Handlers;
using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class PwaInstallInterop : ICommandHandler<InstallPwa>, ICommandHandler<UpdatePwa>
    {
        private readonly IJSRuntime js;
        private static IEventDispatcher events;

        public PwaInstallInterop(IJSRuntime js, ICommandHandlerCollection commandHandlers, IEventDispatcher events)
        {
            Ensure.NotNull(js, "js");
            Ensure.NotNull(commandHandlers, "commandHandlers");
            Ensure.NotNull(events, "events");
            this.js = js;
            PwaInstallInterop.events = events;

            commandHandlers
                .Add<InstallPwa>(this)
                .Add<UpdatePwa>(this);
        }

        [JSInvokable("Pwa.Installable")]
        public static void Installable() 
            => _ = events.PublishAsync(new PwaInstallable());

        [JSInvokable("Pwa.Updateable")]
        public static void Updateable() 
            => _ = events.PublishAsync(new PwaUpdateable());

        async Task ICommandHandler<InstallPwa>.HandleAsync(InstallPwa command) 
            => await js.InvokeVoidAsync("Pwa.Install");

        async Task ICommandHandler<UpdatePwa>.HandleAsync(UpdatePwa command) 
            => await js.InvokeVoidAsync("Pwa.Update");
    }
}
