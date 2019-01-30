using Money.Events;
using Money.Services;
using Money.Services.Globalization;
using Money.Services.Settings;
using Money.Services.Tiles;
using Money.ViewModels;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Bootstrap;
using Neptuo.Commands;
using Neptuo.Converters;
using Neptuo.Data;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Exceptions.Handlers;
using Neptuo.Formatters;
using Neptuo.Formatters.Converters;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Keys;
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
        private readonly IExceptionHandler domainExceptionHandler;
        private readonly string launchArguments;
        private PersistentCommandDispatcher commandDispatcher;
        private PersistentEventDispatcher eventDispatcher;
        private ICompositeTypeProvider typeProvider;
        private DefaultQueryDispatcher queryDispatcher;

        public IRepository<Outcome, IKey> OutcomeRepository { get; private set; }
        public IRepository<Category, IKey> CategoryRepository { get; private set; }
        public IRepository<CurrencyList, IKey> CurrencyListRepository { get; private set; }

        public EntityEventStore EventStore { get; private set; }

        public IFormatter CommandFormatter { get; private set; }
        public IFormatter EventFormatter { get; private set; }

        public static IKey UserKey { get; } = StringKey.Create("tmp_user", "User");

        public BootstrapTask(IExceptionHandler domainExceptionHandler, string launchArguments)
        {
            Ensure.NotNull(domainExceptionHandler, "domainExceptionHandler");
            this.domainExceptionHandler = domainExceptionHandler;
            this.launchArguments = launchArguments;
        }

        public void Initialize()
        {
            ServiceProvider.MessageBuilder = new MessageBuilder();
            ServiceProvider.MainMenuFactory = new MainMenuListFactory();
            ServiceProvider.CurrencyProvider = new DefaultCurrencyProvider();

            StorageFactory storageFactory = new StorageFactory();
            ServiceProvider.EventSourcingContextFactory = storageFactory;
            ServiceProvider.ReadModelContextFactory = storageFactory;
            ServiceProvider.StorageContainerFactory = storageFactory;

            Domain();

            PriceCalculator priceCalculator = new PriceCalculator(eventDispatcher.Handlers);
            ServiceProvider.PriceConverter = priceCalculator;

            ReadModels();

            ServiceProvider.QueryDispatcher = new UserQueryDispatcher(queryDispatcher, () => UserKey);
            ServiceProvider.CommandDispatcher = new UserCommandDispatcher(commandDispatcher, () => UserKey);
            ServiceProvider.EventHandlers = eventDispatcher.Handlers;

            UpgradeService upgradeService = new UpgradeService(ServiceProvider.CommandDispatcher, EventStore, EventFormatter, storageFactory, storageFactory, storageFactory, priceCalculator, () => UserKey);
            ServiceProvider.UpgradeService = upgradeService;

            ServiceProvider.TileService = new TileService();
            ServiceProvider.DevelopmentTools = new DevelopmentService(upgradeService, storageFactory);
            ServiceProvider.RestartService = new RestartService(launchArguments);
            ServiceProvider.AppDataService = new AppDataService(storageFactory, upgradeService);
            ServiceProvider.UserPreferences = new ApplicationSettingsService(new CompositeTypeFormatterFactory(typeProvider), storageFactory);

            CurrencyCache currencyCache = new CurrencyCache(eventDispatcher.Handlers, queryDispatcher);

            using (var eventSourcing = ServiceProvider.EventSourcingContextFactory.Create())
                eventSourcing.Database.EnsureCreated();

            using (var readModels = ServiceProvider.ReadModelContextFactory.Create())
                readModels.Database.EnsureCreated();

            if (ServiceProvider.UpgradeService.IsRequired())
            {
                ServiceProvider.UpgradeService.Completed += async () =>
                {
                    Task delay = Task.Delay(5000);
                    Task currencyCreated = eventDispatcher.Handlers.Await<CurrencyCreated>();

                    await Task.WhenAny(delay, currencyCreated);
                    await Task.Delay(1000);
                    await InitializeCacheAsync(priceCalculator, currencyCache);
                };
            }
            else
            {
               _ = InitializeCacheAsync(priceCalculator, currencyCache);
            }
        }

        private async Task InitializeCacheAsync(PriceCalculator priceCalculator, CurrencyCache currencyCache)
        {
            await currencyCache.InitializeAsync(ServiceProvider.QueryDispatcher);
            await priceCalculator.InitializeAsync(ServiceProvider.QueryDispatcher);
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

            EventStore = new EntityEventStore(ServiceProvider.EventSourcingContextFactory);
            eventDispatcher = new PersistentEventDispatcher(new EmptyEventStore());
            eventDispatcher.DispatcherExceptionHandlers.Add(this);
            eventDispatcher.EventExceptionHandlers.Add(this);

            IFactory<ICompositeStorage> compositeStorageFactory = Factory.Default<JsonCompositeStorage>();

            typeProvider = new ReflectionCompositeTypeProvider(
                new ReflectionCompositeDelegateFactory(),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );

            CommandFormatter = new CompositeCommandFormatter(typeProvider, compositeStorageFactory);
            EventFormatter = new CompositeEventFormatter(typeProvider, compositeStorageFactory, new List<ICompositeFormatterExtender>() { new UserKeyEventExtender() });

            commandDispatcher = new PersistentCommandDispatcher(new SerialCommandDistributor(), new EmptyCommandStore(), CommandFormatter);
            commandDispatcher.CommandExceptionHandlers.Add(domainExceptionHandler);

            OutcomeRepository = new AggregateRootRepository<Outcome>(
                EventStore,
                EventFormatter,
                new ReflectionAggregateRootFactory<Outcome>(),
                eventDispatcher,
                new NoSnapshotProvider(),
                new EmptySnapshotStore()
            );

            CategoryRepository = new AggregateRootRepository<Category>(
                EventStore,
                EventFormatter,
                new ReflectionAggregateRootFactory<Category>(),
                eventDispatcher,
                new NoSnapshotProvider(),
                new EmptySnapshotStore()
            );

            CurrencyListRepository = new AggregateRootRepository<CurrencyList>(
                EventStore,
                EventFormatter,
                new ReflectionAggregateRootFactory<CurrencyList>(),
                eventDispatcher,
                new NoSnapshotProvider(),
                new EmptySnapshotStore()
            );

            Money.BootstrapTask bootstrapTask = new Money.BootstrapTask(
                commandDispatcher.Handlers,
                Factory.Instance(OutcomeRepository),
                Factory.Instance(CategoryRepository),
                Factory.Instance(CurrencyListRepository)
            );
            bootstrapTask.Initialize();
        }

        private void ReadModels()
        {
            // Should match with RecreateReadModelContext.
            queryDispatcher = new DefaultQueryDispatcher();
            Models.Builders.BootstrapTask bootstrapTask = new Models.Builders.BootstrapTask(queryDispatcher, eventDispatcher.Handlers, ServiceProvider.ReadModelContextFactory, ServiceProvider.PriceConverter);
            bootstrapTask.Initialize();
        }

        public void Handle(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
