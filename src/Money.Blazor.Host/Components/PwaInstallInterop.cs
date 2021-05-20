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
        private static bool isInstallable;
        private static bool isUpdateable;

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

            if (isInstallable)
                editor.MakeInstallable();
            else if (isUpdateable)
                editor.MakeUpdateable();
        }

        public void Remove(PwaInstall editor)
        {
            Ensure.NotNull(editor, "editor");
            editors.Remove(editor);
        }

        [JSInvokable("Pwa.Installable")]
        public static void Installable()
        {
            isInstallable = true;
            isUpdateable = false;

            foreach (var editor in editors)
                editor.MakeInstallable();
        }

        [JSInvokable("Pwa.Updateable")]
        public static void Updateable()
        {
            isInstallable = false;
            isUpdateable = true;

            foreach (var editor in editors)
                editor.MakeUpdateable();
        }

        public ValueTask InstallAsync()
            => jSRuntime.InvokeVoidAsync("Pwa.Install");

        public ValueTask UpdateAsync()
            => jSRuntime.InvokeVoidAsync("Pwa.Update");
    }
}
