using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Money.Models;
using Money.Models.Api;
using Money.Services;
using Money.UI.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    internal class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddTransient<Navigator>()
                .AddTransient<ApiClient>()
                .AddSingleton<QueryString>()
                .AddSingleton<CommandMapper>()
                .AddSingleton<QueryMapper>()
                .AddSingleton<ColorCollection>()
                .AddSingleton<IconCollection>();

            Bootstrap.BootstrapTask bootstrapTask = new Bootstrap.BootstrapTask(services);
            bootstrapTask.Initialize();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");

            Interop.ApplicationStarted();
        }
    }
}
