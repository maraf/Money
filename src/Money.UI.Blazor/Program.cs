using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;
using Money.Services;
using System;

namespace Money.UI.Blazor
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new BrowserServiceProvider(services =>
            {
                services.AddTransient<ApiClient>();

                Bootstrap.BootstrapTask bootstrapTask = new Bootstrap.BootstrapTask(services);
                bootstrapTask.Initialize();
            });

            new BrowserRenderer(serviceProvider).AddComponent<App>("app");
        }
    }
}
