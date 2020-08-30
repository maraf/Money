using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Money.EntityFrameworkCore;
using Money.Models;
using Money.Models.Builders;
using Money.Hubs;
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
using Neptuo.Logging;
using Neptuo.Logging.Serialization;
using Neptuo.Logging.Serialization.Filters;
using Neptuo.Logging.Serialization.Formatters;
using Neptuo.Models.Repositories;
using Neptuo.Models.Snapshots;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Money.Bootstrap
{
    public class BootstrapTask : IBootstrapTask
    {
        private readonly IServiceCollection services;
        private readonly IConfiguration configuration;
        private readonly PathResolver pathResolver;

        private ILogFactory logFactory;
        private ILog errorLog;

        private PriceCalculator priceCalculator;
        private ICompositeTypeProvider typeProvider;

        private ExceptionHandlerBuilder exceptionHandlerBuilder;

        private DefaultQueryDispatcher queryDispatcher;
        private PersistentCommandDispatcher commandDispatcher;
        private PersistentEventDispatcher eventDispatcher;

        private EntityEventStore eventStore;

        private IFormatter commandFormatter;
        private IFormatter eventFormatter;
        private IFormatter queryFormatter;
        private IFormatter exceptionFormatter;

        public BootstrapTask(IServiceCollection services, IConfiguration configuration, PathResolver pathResolver)
        {
            Ensure.NotNull(services, "services");
            Ensure.NotNull(configuration, "configuration");
            Ensure.NotNull(pathResolver, "pathResolver");
            this.services = services;
            this.configuration = configuration;
            this.pathResolver = pathResolver;
        }

        public void Initialize()
        {
            Logging();

            exceptionHandlerBuilder = new ExceptionHandlerBuilder();
            exceptionHandlerBuilder.Handler(Handle);

            services
                .AddDbContextWithSchema<ReadModelContext>(configuration.GetSection("ReadModel"), pathResolver)
                .AddDbContextWithSchema<EventSourcingContext>(configuration.GetSection("EventSourcing"), pathResolver)
                .AddSingleton(exceptionHandlerBuilder)
                .AddSingleton<IExceptionHandler>(exceptionHandlerBuilder);

            var provider = services.BuildServiceProvider();

            Domain(provider);

            priceCalculator = new PriceCalculator(eventDispatcher.Handlers, queryDispatcher);

            services
                .AddSingleton(priceCalculator)
                .AddSingleton(new FormatterContainer(commandFormatter, eventFormatter, queryFormatter, exceptionFormatter));

            CreateReadModelContext(provider);
            CreateEventSourcingContext(provider);
            ReadModels(provider);

            services
                .AddSingleton<IEventHandlerCollection>(eventDispatcher.Handlers)
                .AddScoped<ICommandDispatcher>(provider => new UserCommandDispatcher(commandDispatcher, provider.GetService<IHttpContextAccessor>().HttpContext, provider.GetService<ApiHub>()))
                .AddScoped<IQueryDispatcher>(provider => new UserQueryDispatcher(queryDispatcher, provider.GetService<IHttpContextAccessor>().HttpContext));

            CurrencyCache currencyCache = new CurrencyCache(eventDispatcher.Handlers, queryDispatcher, queryDispatcher);

            services
                .AddSingleton(currencyCache);
        }

        private void Logging()
        {
            ILogFilter logFilter = DefaultLogFilter.Debug;

#if !DEBUG
            logFilter = DefaultLogFilter.Warning;
#endif

            logFactory = new DefaultLogFactory("Root")
                .AddSerializer(new ConsoleSerializer(new DefaultLogFormatter(), logFilter));

            errorLog = logFactory.Scope("Error");
        }

        private void Domain(IServiceProvider provider)
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

            var eventSourcingContextFactory = provider.GetRequiredService<IFactory<EventSourcingContext>>();

            eventStore = new EntityEventStore(eventSourcingContextFactory);
            eventDispatcher = new PersistentEventDispatcher(new EmptyEventStore());
            eventDispatcher.DispatcherExceptionHandlers.Add(exceptionHandlerBuilder);
            eventDispatcher.EventExceptionHandlers.Add(exceptionHandlerBuilder);

            IFactory<ICompositeStorage> compositeStorageFactory = Factory.Default<JsonCompositeStorage>();

            typeProvider = new ReflectionCompositeTypeProvider(
                new ReflectionCompositeDelegateFactory(),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );

            commandFormatter = new CompositeCommandFormatter(typeProvider, compositeStorageFactory);
            eventFormatter = new CompositeEventFormatter(typeProvider, compositeStorageFactory, new List<ICompositeFormatterExtender>() { new UserKeyEventExtender() });
            queryFormatter = new CompositeListFormatter(typeProvider, compositeStorageFactory, logFactory);
            exceptionFormatter = new CompositeExceptionFormatter(typeProvider, compositeStorageFactory);

            commandDispatcher = new PersistentCommandDispatcher(new SerialCommandDistributor(), new EmptyCommandStore(), commandFormatter);
            commandDispatcher.DispatcherExceptionHandlers.Add(exceptionHandlerBuilder);
            commandDispatcher.CommandExceptionHandlers.Add(exceptionHandlerBuilder);

            queryDispatcher = new DefaultQueryDispatcher();

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

            UserHandler userHandler = new UserHandler(services.BuildServiceProvider().GetRequiredService<UserManager<User>>(), eventDispatcher);
            commandDispatcher.Handlers.AddAll(userHandler);
            queryDispatcher.AddAll(userHandler);
        }

        private void ReadModels(IServiceProvider provider)
        {
            var readModelContextFactory = provider.GetRequiredService<IFactory<ReadModelContext>>();

            Models.Builders.BootstrapTask bootstrapTask = new Models.Builders.BootstrapTask(
                queryDispatcher,
                eventDispatcher.Handlers,
                readModelContextFactory,
                priceCalculator
            );

            bootstrapTask.Initialize();
        }

        private void CreateEventSourcingContext(IServiceProvider provider)
        {
            var factory = provider.GetRequiredService<IFactory<EventSourcingContext>>();
            using (var eventSourcing = factory.Create())
            {
                eventSourcing.Database.EnsureCreated();
                eventSourcing.Database.Migrate();
            }
        }

        private void CreateReadModelContext(IServiceProvider provider)
        {
            var factory = provider.GetRequiredService<IFactory<ReadModelContext>>();
            using (var readModel = factory.Create())
            {
                readModel.Database.EnsureCreated();
                readModel.Database.Migrate();
            }
        }

        public void Handle(Exception exception)
            => errorLog.Error(exception);
    }
}
