using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
            host.Run();
            serviceProvider = host.Services;
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder().UseBlazorStartup<Startup>();

        internal static void RaiseEvent(string payload) => serviceProvider.GetService<BrowserEventDispatcher>().Raise(payload);
        internal static void RaiseException(string payload) => serviceProvider.GetService<BrowserExceptionHandler>().Raise(payload);

        internal static T Resolve<T>()
            => serviceProvider.GetService<T>();
    }
}
