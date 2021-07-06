using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Money.Api.Routing;
using Money.Components;
using Money.Components.Bootstrap;
using Money.Models;
using Money.Services;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .AddTransient<IComponentActivator, ComponentActivator>();

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

    class ComponentActivator : IComponentActivator
    {
        public IComponent CreateInstance(Type componentType)
        {
            Console.WriteLine($"CA: '{componentType.FullName}'.");

            if (componentType.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
            {
                Console.WriteLine($"CA: '{componentType.FullName}' as 'EventAttachedComponent'.");
                return new EventAttachedComponent() { Type = componentType };
            }
            else if(componentType.IsGenericType && componentType.GetGenericTypeDefinition() == typeof(Wrapped<>))
            {
                componentType = componentType.GetGenericArguments()[0];
            }

            return (IComponent)Activator.CreateInstance(componentType);
        }
    }

    class Wrapped<T> : IComponent
    {
        public void Attach(RenderHandle renderHandle)
        {
            throw new NotImplementedException();
        }

        public Task SetParametersAsync(ParameterView parameters)
        {
            throw new NotImplementedException();
        }
    }

    class EventAttachedComponent : IComponent
    {
        private RenderHandle _renderHandle;
        private RenderFragment _cachedRenderFragment;

        public EventAttachedComponent()
        {
            _cachedRenderFragment = Render;
        }

        public Type Type { get; set; }
        public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
        }

        public Task SetParametersAsync(ParameterView parameters)
        {
            Parameters.Clear();
            foreach (var entry in parameters)
                Parameters[entry.Name] = entry.Value;

            _renderHandle.Render(_cachedRenderFragment);
            return Task.CompletedTask;
        }

        void Render(RenderTreeBuilder builder)
        {
            builder.OpenComponent(0, typeof(Wrapped<>).MakeGenericType(Type));

            if (Parameters != null)
            {
                foreach (var entry in Parameters)
                {
                    builder.AddAttribute(1, entry.Key, entry.Value);
                }
            }

            builder.CloseComponent();
        }
    }
}
