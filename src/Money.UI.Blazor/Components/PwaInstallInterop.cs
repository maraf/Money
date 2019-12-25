using Microsoft.JSInterop;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public class PwaInstallInterop
    {
        private static List<PwaInstall> editors = new List<PwaInstall>();
        private readonly IJSRuntime jSRuntime;

        public PwaInstallInterop(IJSRuntime jSRuntime)
        {
            Ensure.NotNull(jSRuntime, "jSRuntime");
            this.jSRuntime = jSRuntime;
        }

        public void Initialize(PwaInstall editor)
        {
            Ensure.NotNull(editor, "editor");
            editors.Add(editor);
        }

        public void Remove(PwaInstall editor)
        {
            Ensure.NotNull(editor, "editor");
            editors.Remove(editor);
        }

        [JSInvokable("Pwa.Installable")]
        public static void Installable()
        {
            foreach (var editor in editors)
                editor.MakeInstallable();
        }

        [JSInvokable("Pwa.Updateable")]
        public static void Updateable()
        {
            foreach (var editor in editors)
                editor.MakeUpdateable();
        }

        public ValueTask InstallAsync() 
            => jSRuntime.InvokeVoidAsync("BlazorPWA.installPWA");
    }
}
