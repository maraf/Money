using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Money.Components.Bootstrap;
using Money.Models;
using Money.Models.Api;
using Money.Services;
using Money.UI.Blazor;
using Neptuo.Events;
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
                .Configure<ApiClientConfiguration>(BindApiClientConfiguration)
                .AddAuthorizationCore()
                .AddTransient<Interop>()
                .AddTransient<NavigatorUrl>()
                .AddTransient<Navigator>()
                .AddScoped<ApiAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<ApiAuthenticationStateProvider>())
                .AddSingleton<SignalRListener>()
                .AddSingleton<ApiClient>()
                .AddSingleton<ModalInterop>()
                .AddSingleton<TokenContainer>()
                .AddSingleton<QueryString>()
                .AddSingleton<CommandMapper>()
                .AddSingleton<QueryMapper>()
                .AddSingleton<ColorCollection>()
                .AddSingleton<IconCollection>();

            Bootstrap.BootstrapTask bootstrapTask = new Bootstrap.BootstrapTask(services);
            bootstrapTask.Initialize();
        }

        private void BindApiClientConfiguration(ApiClientConfiguration configuration)
        {
#if DEBUG
            configuration.ApiUrl = new Uri("http://localhost:63803", UriKind.Absolute);
#else
            configuration.ApiUrl = new Uri("https://api.money.neptuo.com", UriKind.Absolute);
#endif
        }

        public void Configure(IComponentsApplicationBuilder app, Interop interop)
        {
            app.AddComponent<App>("app");

            interop.ApplicationStarted();

            SignalRListener listener = app.Services.GetService<SignalRListener>();
            app.Services.GetService<IEventHandlerCollection>().AddAll(listener);
        }
    }
}
