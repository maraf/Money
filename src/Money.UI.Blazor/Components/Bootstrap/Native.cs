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
        private Dictionary<string, ModalBase> modals = new Dictionary<string, ModalBase>();

        public Native(IJSRuntime jsRuntime)
        {
            Ensure.NotNull(jsRuntime, "jsRuntime");
            this.jsRuntime = jsRuntime;
        }

        internal void AddModal(string id, ModalBase component)
        {
            modals[id] = component;
            jsRuntime.InvokeAsync<object>("Bootstrap.Modal.Register", id);
        }

        internal void ToggleModal(string id, bool isVisible) 
            => jsRuntime.InvokeAsync<object>("Bootstrap.Modal.Toggle", id, isVisible);

        internal void RemoveModal(string id)
            => modals.Remove(id);

        [JSInvokable]
        public void Bootstrap_ModalHidden(string id)
        {
            ILog log = Program.Resolve<ILogFactory>().Scope("Modal.Native");
            log.Debug($"Modal hidden '{id}'.");
            if (modals.TryGetValue(id, out ModalBase modal))
                modal.MarkAsHidden();
            else
                log.Debug($"Modal not found '{id}'.");
        }
    }
}
