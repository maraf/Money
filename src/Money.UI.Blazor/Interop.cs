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
        public static void ApplicationStarted() 
            => JSRuntime.Current.InvokeAsync<object>("Money.ApplicationStarted");

        public static void NavigateTo(string url)
            => JSRuntime.Current.InvokeAsync<bool>("Money.NavigateTo", url);

        public static void StartSignalR(string url, string token)
            => JSRuntime.Current.InvokeAsync<bool>("Money.StartSignalR", url, token);

        public static void StopSignalR()
            => JSRuntime.Current.InvokeAsync<bool>("Money.StopSignalR");

        public static void SaveToken(string token)
            => JSRuntime.Current.InvokeAsync<bool>("Money.SaveToken", token);

        public static Task<string> LoadTokenAsync()
            => JSRuntime.Current.InvokeAsync<string>("Money.LoadToken");
    }
}
