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
            var serviceProvider = new BrowserServiceProvider(configure =>
            {
                configure.AddTransient<ApiClient>();
            });
             
            new BrowserRenderer(serviceProvider).AddComponent<App>("app");
        }
    }
}
