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

        public async Task ScrollToTopAsync()
            => await jsRuntime.InvokeVoidAsync("window.scrollTo", 0, 0);

        public async Task AnimateSplashAsync()
            => await jsRuntime.InvokeVoidAsync("Money.AnimateSplash");
    }
}
