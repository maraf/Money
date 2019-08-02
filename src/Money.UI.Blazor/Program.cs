using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Neptuo.Events;
using Neptuo.Exceptions;
using System;

namespace Money.UI.Blazor
{
    public class Program
    {
        private static IServiceProvider serviceProvider;

        public static void Main(string[] args)
        {
            IWebAssemblyHost host = CreateHostBuilder(args).Build();
            serviceProvider = host.Services;
            host.Run();
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();

        [JSInvokable]
        public static void RaiseEvent(string payload) => serviceProvider.GetService<BrowserEventDispatcher>().Raise(payload);

        [JSInvokable]
        public static void RaiseException(string payload) => serviceProvider.GetService<BrowserExceptionHandler>().Raise(payload);

        internal static T Resolve<T>() => serviceProvider.GetService<T>();
    }
}
