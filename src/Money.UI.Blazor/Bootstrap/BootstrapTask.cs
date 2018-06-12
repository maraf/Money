using Microsoft.Extensions.DependencyInjection;
using Money.Models.Queries;
using Money.Services;
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
    public class BootstrapTask : IBootstrapTask
    {
        private readonly IServiceCollection services;

        private ILogFactory logFactory;
        //private PriceCalculator priceCalculator;
        private ICompositeTypeProvider typeProvider;

        private IFormatter commandFormatter;
        private IFormatter eventFormatter;
        private IFormatter queryFormatter;
        private IFormatter exceptionFormatter;

        public BootstrapTask(IServiceCollection services)
        {
            Ensure.NotNull(services, "services");
            this.services = services;
        }

        public void Initialize()
        {
            ILogFilter logFilter = DefaultLogFilter.Debug;

#if !DEBUG
            logFilter = DefaultLogFilter.Warning;
#endif

            logFactory = new DefaultLogFactory("Root")
                .AddSerializer(new ConsoleSerializer(new SingleLineLogFormatter(), logFilter));

            Domain();

            //priceCalculator = new PriceCalculator(eventDispatcher.Handlers);
            FormatterContainer formatters = new FormatterContainer(commandFormatter, eventFormatter, queryFormatter, exceptionFormatter);
            BrowserEventDispatcher eventDispatcher = new BrowserEventDispatcher(formatters, logFactory);
            BrowserExceptionHandler exceptionHandler = new BrowserExceptionHandler(formatters, logFactory);

            services
                //.AddSingleton(priceCalculator)
                .AddSingleton(formatters)
                .AddSingleton(logFactory)
                .AddSingleton<MessageBuilder>()
                .AddTransient<ICommandDispatcher, HttpCommandDispatcher>()
                .AddTransient<IQueryDispatcher, HttpQueryDispatcher>()
                .AddTransient<HttpQueryDispatcher.IMiddleware, CategoryNameMiddleware>()
                .AddTransient(typeof(ILog<>), typeof(DefaultLog<>))
                .AddSingleton(eventDispatcher)
                .AddSingleton(eventDispatcher.Handlers)
                .AddSingleton(exceptionHandler)
                .AddSingleton(exceptionHandler.Handler)
                .AddSingleton(exceptionHandler.HandlerBuilder);

            //CurrencyCache currencyCache = new CurrencyCache(eventDispatcher.Handlers, queryDispatcher);

            //services
            //    .AddSingleton(currencyCache);

            //currencyCache.InitializeAsync(queryDispatcher);
            //priceCalculator.InitializeAsync(queryDispatcher);
        }

        private class CategoryNameMiddleware : HttpQueryDispatcher.Middleware<Models.Queries.GetCategoryName, string>
        {
            protected override Task<string> ExecuteAsync(GetCategoryName query, HttpQueryDispatcher.Next next)
                => Task.FromResult("Haha!");
        }

        private void Domain()
        {
            Converts.Repository
                .AddStringTo<int>(Int32.TryParse)
                .AddStringTo<decimal>(Decimal.TryParse)
                .AddStringTo<bool>(Boolean.TryParse)
                .AddEnumSearchHandler(false);
                //.AddJsonEnumSearchHandler()
                //.AddJsonPrimitivesSearchHandler()
                //.AddJsonObjectSearchHandler()
                //.AddJsonKey()
                //.AddJsonTimeSpan()
                //.Add(new ColorConverter())
                //.AddToStringSearchHandler();

            IFactory<ICompositeStorage> compositeStorageFactory = Factory.Getter(() => new SimpleJsonCompositeStorage(logFactory));

            typeProvider = new ReflectionCompositeTypeProvider(
                new ReflectionCompositeDelegateFactory(),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );

            commandFormatter = new CompositeCommandFormatter(typeProvider, compositeStorageFactory);
            eventFormatter = new CompositeEventFormatter(typeProvider, compositeStorageFactory, new List<ICompositeFormatterExtender> () { new UserKeyEventExtender() });
            queryFormatter = new CompositeListFormatter(typeProvider, compositeStorageFactory, logFactory);
            exceptionFormatter = new CompositeExceptionFormatter(typeProvider, compositeStorageFactory);
        }
    }
}
