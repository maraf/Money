using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Money.Api.Routing;
using Money.Components;
using Money.Components.Bootstrap;
using Money.Models;
using Money.Services;
using Neptuo.Events;
using Neptuo.Exceptions;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Money.UI.Blazor
{
    public class Program
    {
        private static Bootstrap.BootstrapTask bootstrapTask;

        public async static Task Main(string[] args)
        {
            // Configure.
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault();
            ConfigureServices(builder.Services);
            ConfigureComponents(builder.RootComponents);

            // Startup.
            WebAssemblyHost host = builder.Build();
            StartupServices(host.Services);

            // Run.
            await host.RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<ApiConfiguration>(configuration =>
                {
#if DEBUG
                    configuration.ApiUrl = new Uri("http://localhost:63803", UriKind.Absolute);
#else
                    configuration.ApiUrl = new Uri("https://api.money.neptuo.com", UriKind.Absolute);
#endif
                })
                .AddAuthorizationCore()
                .AddSingleton(p => new HttpClient() { BaseAddress = p.GetRequiredService<IOptions<ApiConfiguration>>().Value.ApiUrl })
                .AddSingleton<ServerConnectionState>()
                .AddScoped<ApiAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<ApiAuthenticationStateProvider>())
                .AddTransient<ApiTokenValidator>()
                .AddSingleton<SignalRListener>()
                .AddSingleton<ApiHubService>()
                .AddTransient<IApiHubState>(provider => provider.GetRequiredService<ApiHubService>())
                .AddSingleton<ApiVersionChecker>()
                .AddTransient<Interop>()
                .AddSingleton<PwaInstallInterop>()
                .AddTransient<NetworkStateInterop>()
                .AddSingleton<NetworkState>()
                .AddTransient<VisibilityStateInterop>()
                .AddSingleton<VisibilityState>()
                .AddSingleton<PullToRefreshInterop>()
                .AddTransient<CurrencyStorage>()
                .AddTransient<CategoryStorage>()
                .AddTransient<ProfileStorage>()
                .AddTransient<UserPropertyStorage>()
                .AddTransient<NavigatorUrl>()
                .AddSingleton<Navigator>()
                .AddSingleton<Navigator.ModalContainer>()
                .AddScoped<ApiClient>()
                .AddSingleton<ModalInterop>()
                .AddSingleton<TokenContainer>()
                .AddSingleton<QueryString>()
                .AddSingleton<CommandMapper>()
                .AddSingleton<QueryMapper>()
                .AddSingleton<ColorCollection>()
                .AddSingleton<IconCollection>();

            services
                .AddTransient<TokenStorage>()
                .AddBlazoredLocalStorage()
                .AddBlazoredSessionStorage();

            services
                .AddSingleton<TemplateService>();

            bootstrapTask = new Bootstrap.BootstrapTask(services);
            bootstrapTask.Initialize();
        }

        private static void ConfigureComponents(RootComponentMappingCollection rootComponents)
        {
            rootComponents.Add<App>("app");
        }

        private static void StartupServices(IServiceProvider services)
        {
            bootstrapTask.RegisterHandlers(services);

            services.GetRequiredService<IEventHandlerCollection>()
                .AddAll(services.GetRequiredService<SignalRListener>());

            services.GetRequiredService<ApiAuthenticationStateProvider>()
                .AddValidator(services.GetRequiredService<ApiTokenValidator>());

            services.GetRequiredService<VisibilityState>();
        }
    }
}
