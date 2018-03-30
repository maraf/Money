using Microsoft.Extensions.DependencyInjection;
using Money.Internals;
using Money.Services;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Bootstrap;
using Neptuo.Commands;
using Neptuo.Converters;
using Neptuo.Events;
using Neptuo.Exceptions.Handlers;
using Neptuo.Formatters;
using Neptuo.Formatters.Metadata;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Money.Bootstrap
{
    public class BootstrapTask : IBootstrapTask, IExceptionHandler
    {
        private readonly IServiceCollection services;

        //private PriceCalculator priceCalculator;
        private ICompositeTypeProvider typeProvider;

        private IFormatter commandFormatter;
        private IFormatter eventFormatter;
        private IFormatter queryFormatter;

        public BootstrapTask(IServiceCollection services)
        {
            Ensure.NotNull(services, "services");
            this.services = services;
        }

        public void Initialize()
        {
            Domain();

            //priceCalculator = new PriceCalculator(eventDispatcher.Handlers);
            Formatters formatters = new Formatters(commandFormatter, eventFormatter, queryFormatter);
            BrowserEventDispatcher eventDispatcher = new BrowserEventDispatcher(formatters);

            services
                //.AddSingleton(priceCalculator)
                .AddSingleton(formatters)
                .AddTransient<ICommandDispatcher, HttpCommandDispatcher>()
                .AddTransient<IQueryDispatcher, HttpQueryDispatcher>()
                .AddSingleton(eventDispatcher)
                .AddSingleton(eventDispatcher.Handlers);

            //CurrencyCache currencyCache = new CurrencyCache(eventDispatcher.Handlers, queryDispatcher);

            //services
            //    .AddSingleton(currencyCache);

            //currencyCache.InitializeAsync(queryDispatcher);
            //priceCalculator.InitializeAsync(queryDispatcher);
        }

        private void Domain()
        {
            Converts.Repository
                .AddStringTo<int>(Int32.TryParse)
                .AddStringTo<bool>(Boolean.TryParse)
                .AddEnumSearchHandler(false);
                //.AddJsonEnumSearchHandler()
                //.AddJsonPrimitivesSearchHandler()
                //.AddJsonObjectSearchHandler()
                //.AddJsonKey()
                //.AddJsonTimeSpan()
                //.Add(new ColorConverter())
                //.AddToStringSearchHandler();

            IFactory<ICompositeStorage> compositeStorageFactory = Factory.Default<JsonCompositeStorage>();

            typeProvider = new ReflectionCompositeTypeProvider(
                new ReflectionCompositeDelegateFactory(),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );

            commandFormatter = new CompositeCommandFormatter(typeProvider, compositeStorageFactory);
            eventFormatter = new CompositeEventFormatter(typeProvider, compositeStorageFactory);
            queryFormatter = new CompositeListFormatter(typeProvider, compositeStorageFactory);
        }

        public void Handle(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
