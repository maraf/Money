using Microsoft.EntityFrameworkCore;
using Money.Commands;
using Money.Data;
using Money.Events;
using Money.Models.Builders;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Commands;
using Neptuo.Data;
using Neptuo.Events;
using Neptuo.Formatters;
using Neptuo.Migrations;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using Neptuo.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using EventEntity = Neptuo.Data.Entity.EventEntity;

namespace Money.Bootstrap
{
    internal class UpgradeService : UpgradeServiceBase
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IEventRebuilderStore eventStore;
        private readonly IFormatter eventFormatter;
        private readonly IFactory<EventSourcingContext> eventSourceContextFactory;
        private readonly IFactory<ReadModelContext> readModelContextFactory;
        private readonly IPriceConverter priceConverter;
        private readonly Func<IKey> userKeyGetter;

        public const int CurrentVersion = 6;

        public UpgradeService(ICommandDispatcher commandDispatcher, IEventRebuilderStore eventStore, IFormatter eventFormatter, IFactory<EventSourcingContext> eventSourceContextFactory, IFactory<ReadModelContext> readModelContextFactory, IFactory<ApplicationDataContainer> storageContainerFactory, IPriceConverter priceConverter, Func<IKey> userKeyGetter)
            : base(storageContainerFactory, CurrentVersion)
        {
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(eventStore, "eventStore");
            Ensure.NotNull(eventFormatter, "eventFormatter");
            Ensure.NotNull(eventSourceContextFactory, "eventSourceContextFactory");
            Ensure.NotNull(readModelContextFactory, "readModelContextFactory");
            Ensure.NotNull(priceConverter, "priceConverter");
            Ensure.NotNull(userKeyGetter, "userKeyGetter");
            this.commandDispatcher = commandDispatcher;
            this.eventStore = eventStore;
            this.eventFormatter = eventFormatter;
            this.eventSourceContextFactory = eventSourceContextFactory;
            this.readModelContextFactory = readModelContextFactory;
            this.priceConverter = priceConverter;
            this.userKeyGetter = userKeyGetter;
        }

        protected override async Task FirstRunAsync(IUpgradeContext context, int currentVersion)
        {
            context.StartingStep(0 - currentVersion, "Creating initial data.");
            EnsureReadModelDatabase();
            EnsureEventSourcingDatabase();
            await CreateDefaultCategoriesAsync();
            await CreateDefaultCurrenciesAsync();
        }

        protected override async Task UpgradeOverrideAsync(IUpgradeContext context, int currentVersion)
        {
            if (currentVersion < 1)
            {
                context.StartingStep(0 - currentVersion, "Creating default categories.");
                EnsureReadModelDatabase();
                EnsureEventSourcingDatabase();
                await UpgradeVersion1();
            }

            if (currentVersion < 2)
            {
                context.StartingStep(1 - currentVersion, "Rebuilding internal database.");
                await UpgradeVersion2();
            }

            if (currentVersion < 3)
            {
                context.StartingStep(2 - currentVersion, "Creating default currencies.");
                await UpgradeVersion3();
            }

            if (currentVersion < 4)
            {
                context.StartingStep(3 - currentVersion, "Adding support for category icons.");
                await UpgradeVersion4();
            }

            if (currentVersion < 5)
            {
                context.StartingStep(4 - currentVersion, "Adding user info to entries.");
                await UpgradeVersion5();
            }

            if (currentVersion < 6)
            {
                context.StartingStep(1 - currentVersion, "Rebuilding internal database.");
                await UpgradeVersion6();
            }
        }

        private async Task UpgradeVersion1()
        {
            RecreateEventSourcingContext();
            await RecreateReadModelContextAsync();
            await CreateDefaultCategoriesAsync();
        }

        private async Task CreateDefaultCategoriesAsync()
        {
            await commandDispatcher.HandleAsync(new CreateCategory("Bills", "Regular expenses", ColorConverter.Map(Colors.DarkGreen)));
            await commandDispatcher.HandleAsync(new CreateCategory("Home", "Do it yourself", ColorConverter.Map(Colors.SandyBrown)));
            await commandDispatcher.HandleAsync(new CreateCategory("Food", "Home cooked meals", ColorConverter.Map(Colors.OrangeRed)));
            await commandDispatcher.HandleAsync(new CreateCategory("Eating Out", "Outsourcing food creation", ColorConverter.Map(Colors.DarkRed)));
        }

        private Task UpgradeVersion2()
        {
            return RecreateReadModelContextAsync();
        }

        private async Task UpgradeVersion3()
        {
            await RecreateReadModelContextAsync();
            await CreateDefaultCurrenciesAsync();
        }

        private async Task CreateDefaultCurrenciesAsync()
        {
            await commandDispatcher.HandleAsync(new CreateCurrency("USD", "$"));
        }

        private void RecreateEventSourcingContext()
        {
            using (var eventSourcing = eventSourceContextFactory.Create())
            {
                eventSourcing.Database.EnsureDeleted();
                eventSourcing.Database.EnsureCreated();
                eventSourcing.Database.Migrate();
            }
        }

        internal async Task RecreateReadModelContextAsync()
        {
            using (var readModels = readModelContextFactory.Create())
            {
                await readModels.Database.EnsureDeletedAsync();
                await readModels.Database.EnsureCreatedAsync();
            }

            // Should match with ReadModels.
            Rebuilder rebuilder = new Rebuilder(eventStore, eventFormatter);
            DefaultQueryDispatcher queryDispatcher = new DefaultQueryDispatcher();

            Models.Builders.BootstrapTask bootstrapTask = new Models.Builders.BootstrapTask(queryDispatcher, rebuilder, readModelContextFactory, priceConverter);
            bootstrapTask.Initialize();

            await rebuilder.RunAsync();
        }

        private void EnsureReadModelDatabase(ReadModelContext readModels = null)
        {
            if (readModels == null)
            {
                using (readModels = readModelContextFactory.Create())
                {
                    readModels.Database.EnsureCreated();
                    readModels.Database.Migrate();
                }
            }
            else
            {
                readModels.Database.EnsureCreated();
                readModels.Database.Migrate();
            }
        }

        private void EnsureEventSourcingDatabase(EventSourcingContext eventSourcing = null)
        {
            if (eventSourcing == null)
            {
                using (eventSourcing = eventSourceContextFactory.Create())
                {
                    eventSourcing.Database.EnsureCreated();
                    eventSourcing.Database.Migrate();
                }
            }
            else
            {
                eventSourcing.Database.EnsureCreated();
                eventSourcing.Database.Migrate();
            }
        }

        private async Task UpgradeVersion4()
        {
            await RecreateReadModelContextAsync();
        }

        private async Task UpgradeVersion5()
        {
            EnsureReadModelDatabase();

            using (var eventSourcing = eventSourceContextFactory.Create())
            {
                List<EventEntity> entities = await eventSourcing.Events.ToListAsync();
                foreach (EventEntity entity in entities)
                {
                    EventModel model = entity.ToModel();
                    IEvent payload = (IEvent)eventFormatter.Deserialize(Type.GetType(model.EventKey.Type), model.Payload);

                    if (payload is UserEvent userEvent && (userEvent.UserKey == null || userEvent.UserKey.IsEmpty))
                    {
                        userEvent.UserKey = userKeyGetter();
                        entity.Payload = eventFormatter.Serialize(userEvent);
                    }
                }

                await eventSourcing.SaveAsync();
            }

            await RecreateReadModelContextAsync();
        }

        private async Task UpgradeVersion6()
        {
            await RecreateReadModelContextAsync();
        }
    }
}
