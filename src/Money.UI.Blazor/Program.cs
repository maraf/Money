using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Money.Components;
using Money.Components.Bootstrap;
using Money.Models;
using Money.Models.Api;
using Money.Services;
using Neptuo.Events;
using Neptuo.Exceptions;
using System;
using System.Threading.Tasks;

namespace Money.UI.Blazor
{
    public class Program
    {
        private static IServiceProvider serviceProvider;
        private static Bootstrap.BootstrapTask bootstrapTask;

        public async static Task Main(string[] args)
        {
            // Create builder.
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault();
            builder.RootComponents.Add<App>("app");
            ConfigureServices(builder.Services);

            // Run application.
            WebAssemblyHost host = builder.Build();
            serviceProvider = host.Services;
            bootstrapTask.RegisterHandlers(serviceProvider);
            await host.RunAsync();
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<ApiClientConfiguration>(BindApiClientConfiguration)
                .AddSingleton<ApiHubService>()
                .AddTransient<Interop>()
                .AddSingleton<PwaInstallInterop>()
                .AddTransient<NetworkStateInterop>()
                .AddSingleton<NetworkState>()
                .AddTransient<CurrencyStorage>()
                .AddTransient<CategoryStorage>()
                .AddTransient<ProfileStorage>()
                .AddTransient<NavigatorUrl>()
                .AddSingleton<Navigator>()
                .AddSingleton<ApiClient>()
                .AddSingleton<ModalInterop>()
                .AddSingleton<TokenContainer>()
                .AddSingleton<QueryString>()
                .AddSingleton<CommandMapper>()
                .AddSingleton<QueryMapper>()
                .AddSingleton<ColorCollection>()
                .AddSingleton<IconCollection>();

            bootstrapTask = new Bootstrap.BootstrapTask(services);
            bootstrapTask.Initialize();
        }

        private static void BindApiClientConfiguration(ApiClientConfiguration configuration)
        {
#if DEBUG
            configuration.ApiUrl = new Uri("http://localhost:63803", UriKind.Absolute);
#else
            configuration.ApiUrl = new Uri("https://api.money.neptuo.com", UriKind.Absolute);
#endif
        }

        [JSInvokable]
        public static void RaiseEvent(string payload) => serviceProvider.GetService<BrowserEventDispatcher>().Raise(payload);

        [JSInvokable]
        public static void RaiseException(string payload) => serviceProvider.GetService<BrowserExceptionHandler>().Raise(payload);

        internal static T Resolve<T>() => serviceProvider.GetService<T>();
    }
}
