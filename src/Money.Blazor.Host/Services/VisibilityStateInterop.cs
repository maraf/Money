using Microsoft.JSInterop;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class VisibilityStateInterop
    {
        private readonly IJSRuntime jsRuntime;
        private Action<bool> handler;

        public VisibilityStateInterop(IJSRuntime jsRuntime)
        {
            Ensure.NotNull(jsRuntime, "jsRuntime");
            this.jsRuntime = jsRuntime;
        }

        public ValueTask InitializeAsync(Action<bool> handler)
        {
            Ensure.NotNull(handler, "handler");
            this.handler = handler;

            return jsRuntime.InvokeVoidAsync("Visibility.Initialize", DotNetObjectReference.Create(this));
        }

        [JSInvokable("Visibility.StatusChanged")]
        public void OnStatusChanged(bool isOnline)
            => handler?.Invoke(isOnline);
    }
}
