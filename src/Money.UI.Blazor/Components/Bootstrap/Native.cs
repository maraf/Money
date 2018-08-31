using Microsoft.JSInterop;
using Money.UI.Blazor;
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
        private static Dictionary<string, ModalBase> modals = new Dictionary<string, ModalBase>();

        internal static void AddModal(string id, ModalBase component)
        {
            modals[id] = component;
            JSRuntime.Current.InvokeAsync<object>("Bootstrap.Modal.Register", id);
        }

        internal static void RemoveModal(string id)
            => modals.Remove(id);

        [JSInvokable]
        public static void Bootstrap_ModalHidden(string id)
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
