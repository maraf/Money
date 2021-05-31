using Microsoft.Extensions.DependencyInjection;
using Money.Commands;
using Money.Models.Queries;
using Money.Services;
using Money.Services.Converters;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Bootstrap;
using Neptuo.Commands;
using Neptuo.Converters;
using Neptuo.Events;
using Neptuo.Exceptions;
using Neptuo.Formatters;
using Neptuo.Formatters.Metadata;
using Neptuo.Logging;
using Neptuo.Logging.Serialization;
using Neptuo.Logging.Serialization.Filters;
using Neptuo.Logging.Serialization.Formatters;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Money.Bootstrap
{
    internal class BootstrapTask : IBootstrapTask
    {
        private readonly IServiceCollection services;

        private ILogFactory logFactory;
        //private PriceCalculator priceCalculator;
        private ICompositeTypeProvider typeProvider;

        private IFormatter commandFormatter;
        private IFormatter eventFormatter;
        private IFormatter queryFormatter;
        private IFormatter exceptionFormatter;

        private BrowserEventDispatcher eventDispatcher;

        public BootstrapTask(IServiceCollection services)
        {
            Ensure.NotNull(services, "services");
            this.services = services;
        }

        public void Initialize()
        {
            ILogFilter logFilter = DefaultLogFilter.Warning;

#if DEBUG
            logFilter = PrefixLogFilter.Ignored(new[] {
                "Root.Json",
                "Root.CompositeListFormatter",
                "Root.ApiClient",
                "Root.ApiAuthenticationState"
            });
#endif

            logFactory = new DefaultLogFactory("Root")
                .AddSerializer(new ConsoleSerializer(new SingleLineLogFormatter(), logFilter));

            Json json = new Json();

            Domain(json);

            //priceCalculator = new PriceCalculator(eventDispatcher.Handlers);
            FormatterContainer formatters = new FormatterContainer(commandFormatter, eventFormatter, queryFormatter, exceptionFormatter);
            eventDispatcher = new BrowserEventDispatcher(formatters, logFactory, json);
            BrowserExceptionHandler exceptionHandler = new BrowserExceptionHandler(formatters, logFactory, json);

            services
                //.AddSingleton(priceCalculator)
                .AddSingleton(json)
                .AddSingleton(formatters)
                .AddSingleton(logFactory)
                .AddTransient<CurrencyFormatterFactory>()
                .AddSingleton<MessageBuilder>()
                .AddScoped<LocalExpenseOnlineRunner>()
                .AddTransient<HttpCommandDispatcher>()
                .AddTransient<CommandStorage>()
                .AddTransient<CreateExpenseStorage>()
                .AddTransient<OfflineCommandDispatcher>()
                .AddSingleton<LocalCommandDispatcher>()
                .AddSingleton<MenuItemService>()
                .AddSingleton<ICommandHandlerCollection, LocalCommandHandlerCollection>()
                .AddTransient<ICommandDispatcher, LocalCommandDispatcher>()
                .AddTransient<IQueryDispatcher, HttpQueryDispatcher>()
                .AddTransient(typeof(ILog<>), typeof(DefaultLog<>))
                .AddSingleton(eventDispatcher)
                .AddSingleton(eventDispatcher.Handlers)
                .AddSingleton(eventDispatcher.Dispatcher)
                .AddSingleton(exceptionHandler)
                .AddSingleton(exceptionHandler.Handler)
                .AddSingleton(exceptionHandler.HandlerBuilder);

            void AddMiddleware<T>(IServiceCollection services, bool register = true)
                where T : class, HttpQueryDispatcher.IMiddleware
            {
                if (register)
                    services.AddScoped<T>();

                services.AddTransient<HttpQueryDispatcher.IMiddleware>(sp => sp.GetService<T>());
            }

            AddMiddleware<CategoryMiddleware>(services);
            AddMiddleware<CurrencyMiddleware>(services);
            AddMiddleware<UserMiddleware>(services);
            AddMiddleware<UserPropertyMiddleware>(services);
            AddMiddleware<ApiVersionChecker>(services);
            AddMiddleware<UserPropertyQueryHandler>(services);
            AddMiddleware<PwaInstallInterop>(services, register: false);
            AddMiddleware<MenuItemService>(services, register: false);

            //CurrencyCache currencyCache = new CurrencyCache(eventDispatcher.Handlers, queryDispatcher);

            //services
            //    .AddSingleton(currencyCache);

            //currencyCache.InitializeAsync(queryDispatcher);
            //priceCalculator.InitializeAsync(queryDispatcher);
        }

        private void Domain(Json json)
        {
            Converts.Repository
                .AddStringTo<int>(Int32.TryParse)
                .AddStringTo<decimal>(Decimal.TryParse)
                .AddStringTo<bool>(Boolean.TryParse)
                .Add(new VersionConverter())
                .AddEnumSearchHandler(false);
                //.AddJsonEnumSearchHandler()
                //.AddJsonPrimitivesSearchHandler()
                //.AddJsonObjectSearchHandler()
                //.AddJsonKey()
                //.AddJsonTimeSpan()
                //.Add(new ColorConverter())
                //.AddToStringSearchHandler();

            IFactory<ICompositeStorage> compositeStorageFactory = Factory.Getter(() => new SystemJsonCompositeStorage(logFactory, json));

            typeProvider = new ReflectionCompositeTypeProvider(
                new ReflectionCompositeDelegateFactory(),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );

            commandFormatter = new CompositeCommandFormatter(typeProvider, compositeStorageFactory);
            eventFormatter = new CompositeEventFormatter(typeProvider, compositeStorageFactory, new List<ICompositeFormatterExtender> () { new UserKeyEventExtender() });
            queryFormatter = new CompositeListFormatter(typeProvider, compositeStorageFactory, logFactory);
            exceptionFormatter = new CompositeExceptionFormatter(typeProvider, compositeStorageFactory);
        }

        internal void RegisterHandlers(IServiceProvider serviceProvider)
        {
            eventDispatcher.Handlers.AddAll(serviceProvider.GetRequiredService<CategoryMiddleware>());
            eventDispatcher.Handlers.AddAll(serviceProvider.GetRequiredService<CurrencyMiddleware>());
            eventDispatcher.Handlers.AddAll(serviceProvider.GetRequiredService<UserMiddleware>());
            eventDispatcher.Handlers.AddAll(serviceProvider.GetRequiredService<UserPropertyMiddleware>());

            serviceProvider.GetService<LocalExpenseOnlineRunner>().Initialize();
        }
    }
}
