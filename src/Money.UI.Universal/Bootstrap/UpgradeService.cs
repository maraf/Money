using Microsoft.EntityFrameworkCore;
using Money.Commands;
using Money.Data;
using Money.Models.Builders;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Commands;
using Neptuo.Data;
using Neptuo.Events;
using Neptuo.Formatters;
using Neptuo.Migrations;
using Neptuo.Queries;
using Neptuo.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

namespace Money.Bootstrap
{
    internal class UpgradeService : IUpgradeService
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IEventRebuilderStore eventStore;
        private readonly IFormatter eventFormatter;
        private readonly IFactory<EventSourcingContext> eventSourceContextFactory;
        private readonly IFactory<ReadModelContext> readModelContextFactory;
        private readonly IFactory<ApplicationDataContainer> storageContainerFactory;
        private readonly IPriceConverter priceConverter;

        public const int CurrentVersion = 4;

        public UpgradeService(ICommandDispatcher commandDispatcher, IEventRebuilderStore eventStore, IFormatter eventFormatter, IFactory<EventSourcingContext> eventSourceContextFactory, IFactory<ReadModelContext> readModelContextFactory, IFactory<ApplicationDataContainer> storageContainerFactory, IPriceConverter priceConverter)
        {
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(eventStore, "eventStore");
            Ensure.NotNull(eventFormatter, "eventFormatter");
            Ensure.NotNull(eventSourceContextFactory, "eventSourceContextFactory");
            Ensure.NotNull(readModelContextFactory, "readModelContextFactory");
            Ensure.NotNull(storageContainerFactory, "storageContainerFactory");
            Ensure.NotNull(priceConverter, "priceConverter");
            this.commandDispatcher = commandDispatcher;
            this.eventStore = eventStore;
            this.eventFormatter = eventFormatter;
            this.eventSourceContextFactory = eventSourceContextFactory;
            this.readModelContextFactory = readModelContextFactory;
            this.storageContainerFactory = storageContainerFactory;
            this.priceConverter = priceConverter;
        }

        public bool IsRequired()
        {
            return CurrentVersion > GetCurrentVersion();
        }

        public async Task UpgradeAsync(IUpgradeContext context)
        {
            int currentVersion = GetCurrentVersion();
            if (CurrentVersion <= currentVersion)
                return;

            context.TotalSteps(CurrentVersion - currentVersion);
            await Task.Delay(500);

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

            ApplicationDataContainer migrationContainer = GetMigrationContainer();
            migrationContainer.Values["Version"] = CurrentVersion;
        }

        private ApplicationDataContainer GetMigrationContainer()
        {
            ApplicationDataContainer root = storageContainerFactory.Create();

            ApplicationDataContainer migrationContainer;
            if (!root.Containers.TryGetValue("Migration", out migrationContainer))
                migrationContainer = root.CreateContainer("Migration", ApplicationDataCreateDisposition.Always);

            return migrationContainer;
        }

        private int GetCurrentVersion()
        {
            ApplicationDataContainer migrationContainer = GetMigrationContainer();
            int currentVersion = (int?)migrationContainer.Values["Version"] ?? 0;
            return currentVersion;
        }

        private async Task UpgradeVersion1()
        {
            RecreateEventSourcingContext();
            await RecreateReadModelContextAsync();

            await commandDispatcher.HandleAsync(new CreateCategory("Home", "DIY", ColorConverter.Map(Colors.SandyBrown)));
            await commandDispatcher.HandleAsync(new CreateCategory("Food", "Home boilt stuff", ColorConverter.Map(Colors.OrangeRed)));
            await commandDispatcher.HandleAsync(new CreateCategory("Eating Out", "Outsourced", ColorConverter.Map(Colors.DarkRed)));
        }

        private Task UpgradeVersion2()
        {
            return RecreateReadModelContextAsync();
        }

        private async Task UpgradeVersion3()
        {
            await RecreateReadModelContextAsync();
            await commandDispatcher.HandleAsync(new CreateCurrency("CZK", "Kč"));
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

        internal Task RecreateReadModelContextAsync()
        {
            using (var readModels = readModelContextFactory.Create())
            {
                readModels.Database.EnsureDeleted();
                readModels.Database.EnsureCreated();
            }

            // Should match with ReadModels.
            Rebuilder rebuilder = new Rebuilder(eventStore, eventFormatter);
            DefaultQueryDispatcher queryDispatcher = new DefaultQueryDispatcher();

            Models.Builders.BootstrapTask bootstrapTask = new Models.Builders.BootstrapTask(queryDispatcher, rebuilder, readModelContextFactory, priceConverter);
            bootstrapTask.Initialize();

            return rebuilder.RunAsync();
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
    }
}
