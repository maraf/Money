using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Browser.Interop;
using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;
using Money.Internals;
using Money.Services;
using System;

namespace Money.UI.Blazor
{
    public class Program
    {
        private static IServiceProvider serviceProvider;

        private static void Main(string[] args)
        {
            serviceProvider = new BrowserServiceProvider(services =>
            {
                services.AddTransient<ApiClient>();

                Bootstrap.BootstrapTask bootstrapTask = new Bootstrap.BootstrapTask(services);
                bootstrapTask.Initialize();
            });

            new BrowserRenderer(serviceProvider).AddComponent<App>("app");

            RegisteredFunction.Invoke<object>("ApplicationStarted", new object[0]);
        }

        internal static void RaiseEvent(string payload) => serviceProvider.GetService<BrowserEventDispatcher>().Raise(payload);
        internal static void RaiseException(string payload) => serviceProvider.GetService<BrowserExceptionHandler>().Raise(payload);
    }
}
