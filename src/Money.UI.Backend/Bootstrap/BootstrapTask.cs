using Microsoft.Extensions.DependencyInjection;
using Money.Data;
using Money.Services;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Bootstrap;
using Neptuo.Commands;
using Neptuo.Converters;
using Neptuo.Data;
using Neptuo.Events;
using Neptuo.Exceptions.Handlers;
using Neptuo.Formatters;
using Neptuo.Formatters.Converters;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Repositories;
using Neptuo.Models.Snapshots;
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

        private IFactory<ReadModelContext> readModelContextFactory;
        private IFactory<EventSourcingContext> eventSourcingContextFactory;

        private PriceCalculator priceCalculator;
        private ICompositeTypeProvider typeProvider;

        private DefaultQueryDispatcher queryDispatcher;
        private PersistentCommandDispatcher commandDispatcher;
        private PersistentEventDispatcher eventDispatcher;

        private EntityEventStore eventStore;

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
            readModelContextFactory = Factory.Getter(() => new ReadModelContext("Filename=ReadModel.db"));
            eventSourcingContextFactory = Factory.Getter(() => new EventSourcingContext("Filename=EventSourcing.db"));
            CreateReadModelContext();
            CreateEventSourcingContext();

            services
                .AddSingleton(readModelContextFactory)
                .AddSingleton(eventSourcingContextFactory);

            Domain();

            priceCalculator = new PriceCalculator(eventDispatcher.Handlers);

            services
                .AddSingleton(priceCalculator)
                .AddSingleton(new FormatterContainer(commandFormatter, eventFormatter, queryFormatter, exceptionFormatter));

            ReadModels();

            services
                .AddSingleton<IQueryDispatcher>(queryDispatcher)
                .AddSingleton<IEventHandlerCollection>(eventDispatcher.Handlers)
                .AddSingleton<ICommandDispatcher>(commandDispatcher);

            CurrencyCache currencyCache = new CurrencyCache(eventDispatcher.Handlers, queryDispatcher);

            services
                .AddSingleton(currencyCache);

            currencyCache.InitializeAsync(queryDispatcher);
            priceCalculator.InitializeAsync(queryDispatcher);
        }

        private void Domain()
        {
            Converts.Repository
                .AddStringTo<int>(Int32.TryParse)
                .AddStringTo<bool>(Boolean.TryParse)
                .AddEnumSearchHandler(false)
                .AddJsonEnumSearchHandler()
                .AddJsonPrimitivesSearchHandler()
                .AddJsonObjectSearchHandler()
                .AddJsonKey()
                .AddJsonTimeSpan()
                .Add(new ColorConverter())
                .AddToStringSearchHandler();

            eventStore = new EntityEventStore(eventSourcingContextFactory);
            eventDispatcher = new PersistentEventDispatcher(new EmptyEventStore());
            eventDispatcher.DispatcherExceptionHandlers.Add(this);
            eventDispatcher.EventExceptionHandlers.Add(this);

            IFactory<ICompositeStorage> compositeStorageFactory = Factory.Default<JsonCompositeStorage>();

            typeProvider = new ReflectionCompositeTypeProvider(
                new ReflectionCompositeDelegateFactory(),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );

            commandFormatter = new CompositeCommandFormatter(typeProvider, compositeStorageFactory);
            eventFormatter = new CompositeEventFormatter(typeProvider, compositeStorageFactory);
            queryFormatter = new CompositeListFormatter(typeProvider, compositeStorageFactory);
            exceptionFormatter = new CompositeTypeFormatter(typeProvider, compositeStorageFactory);

            commandDispatcher = new PersistentCommandDispatcher(new SerialCommandDistributor(), new EmptyCommandStore(), commandFormatter);

            var outcomeRepository = new AggregateRootRepository<Outcome>(
                eventStore,
                eventFormatter,
                new ReflectionAggregateRootFactory<Outcome>(),
                eventDispatcher,
                new NoSnapshotProvider(),
                new EmptySnapshotStore()
            );

            var categoryRepository = new AggregateRootRepository<Category>(
                eventStore,
                eventFormatter,
                new ReflectionAggregateRootFactory<Category>(),
                eventDispatcher,
                new NoSnapshotProvider(),
                new EmptySnapshotStore()
            );

            var currencyListRepository = new AggregateRootRepository<CurrencyList>(
                eventStore,
                eventFormatter,
                new ReflectionAggregateRootFactory<CurrencyList>(),
                eventDispatcher,
                new NoSnapshotProvider(),
                new EmptySnapshotStore()
            );

            Money.BootstrapTask bootstrapTask = new Money.BootstrapTask(
                commandDispatcher.Handlers,
                Factory.Instance(outcomeRepository),
                Factory.Instance(categoryRepository),
                Factory.Instance(currencyListRepository)
            );

            bootstrapTask.Initialize();
        }

        private void ReadModels()
        {
            queryDispatcher = new DefaultQueryDispatcher();

            Models.Builders.BootstrapTask bootstrapTask = new Models.Builders.BootstrapTask(
                queryDispatcher,
                eventDispatcher.Handlers,
                readModelContextFactory,
                priceCalculator
            );

            bootstrapTask.Initialize();
        }

        private void CreateEventSourcingContext()
        {
            using (var eventSourcing = eventSourcingContextFactory.Create())
                eventSourcing.Database.EnsureCreated();
        }

        private void CreateReadModelContext()
        {
            using (var readModels = readModelContextFactory.Create())
                readModels.Database.EnsureCreated();
        }

        public void Handle(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
