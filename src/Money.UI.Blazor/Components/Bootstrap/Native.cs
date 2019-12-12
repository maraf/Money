using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Money.UI.Blazor;
using Neptuo;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components.Bootstrap
{
    public class Native
    {
        private readonly IJSRuntime jsRuntime;
        private static Dictionary<string, Modal> modals = new Dictionary<string, Modal>();

        public Native(IJSRuntime jsRuntime)
        {
            Ensure.NotNull(jsRuntime, "jsRuntime");
            this.jsRuntime = jsRuntime;
        }

        internal void AddModal(string id, Modal component)
        {
            modals[id] = component;
            jsRuntime.InvokeAsync<object>("Bootstrap.Modal.Register", id);
        }

        internal void ToggleModal(string id, bool isVisible) 
            => jsRuntime.InvokeAsync<object>("Bootstrap.Modal.Toggle", id, isVisible);

        internal void RemoveModal(string id)
            => modals.Remove(id);

        [JSInvokable]
        public static void Bootstrap_ModalHidden(string id)
        {
            ILog log = Program.Resolve<ILogFactory>().Scope("Modal.Native");
            log.Debug($"Modal hidden '{id}'.");
            if (modals.TryGetValue(id, out Modal modal))
                modal.MarkAsHidden();
            else
                log.Debug($"Modal not found '{id}'.");
        }
    }
}
