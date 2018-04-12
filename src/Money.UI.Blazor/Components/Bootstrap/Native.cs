using Microsoft.AspNetCore.Blazor.Browser.Interop;
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
            RegisteredFunction.Invoke<object>("Bootstrap_Modal_Register", id);
        }

        internal static void RemoveModal(string id)
            => modals.Remove(id);

        internal static void ModalHidden(string id)
        {
            Console.WriteLine($"Modal hidden '{id}'.");
            if (modals.TryGetValue(id, out ModalBase modal))
                modal.IsVisible = false;
            else
                Console.WriteLine($"Modal not found '{id}'.");
        }
    }
}
