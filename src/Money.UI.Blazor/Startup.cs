using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Money.Models;
using Money.Services;
using Money.UI.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddTransient<ApiClient>()
                .AddSingleton<ColorCollection>();

            Bootstrap.BootstrapTask bootstrapTask = new Bootstrap.BootstrapTask(services);
            bootstrapTask.Initialize();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");

            JSRuntime.Current.InvokeAsync<object>("Money.ApplicationStarted");
        }
    }
}
