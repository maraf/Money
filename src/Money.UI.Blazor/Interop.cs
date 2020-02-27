using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    public class Interop
    {
        private readonly IJSRuntime jsRuntime;

        public Interop(IJSRuntime jsRuntime)
        {
            Ensure.NotNull(jsRuntime, "jsRuntime");
            this.jsRuntime = jsRuntime;
        }

        public void ApplicationStarted() 
            => jsRuntime.InvokeAsync<object>("Money.ApplicationStarted");

        public void NavigateTo(string url)
            => jsRuntime.InvokeAsync<bool>("Money.NavigateTo", url);

        public void SaveToken(string token)
            => jsRuntime.InvokeAsync<bool>("Money.SaveToken", token);

        public ValueTask<string> LoadTokenAsync()
            => jsRuntime.InvokeAsync<string>("Money.LoadToken");
    }
}
