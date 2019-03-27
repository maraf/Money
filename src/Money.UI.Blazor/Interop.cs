using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    internal class Interop
    {
        [Inject]
        internal IJSRuntime JSRuntime { get; set; }

        public void ApplicationStarted() 
            => JSRuntime.InvokeAsync<object>("Money.ApplicationStarted");

        public void NavigateTo(string url)
            => JSRuntime.InvokeAsync<bool>("Money.NavigateTo", url);

        public void StartSignalR(string url, string token)
            => JSRuntime.InvokeAsync<bool>("Money.StartSignalR", url, token);

        public void StopSignalR()
            => JSRuntime.InvokeAsync<bool>("Money.StopSignalR");

        public void SaveToken(string token)
            => JSRuntime.InvokeAsync<bool>("Money.SaveToken", token);

        public Task<string> LoadTokenAsync()
            => JSRuntime.InvokeAsync<string>("Money.LoadToken");
    }
}
