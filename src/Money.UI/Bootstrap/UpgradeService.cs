using Microsoft.EntityFrameworkCore;
using Money.Data;
using Money.Services;
using Money.Services.Models.Builders;
using Neptuo;
using Neptuo.Data;
using Neptuo.Events;
using Neptuo.Formatters;
using Neptuo.Migrations;
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
        private readonly IDomainFacade domainFacade;
        private readonly IEventRebuilderStore eventStore;
        private readonly IFormatter eventFormatter;

        public const int CurrentVersion = 2;

        public UpgradeService(IDomainFacade domainFacade, IEventRebuilderStore eventStore, IFormatter eventFormatter)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(eventStore, "eventStore");
            Ensure.NotNull(eventFormatter, "eventFormatter");
            this.domainFacade = domainFacade;
            this.eventStore = eventStore;
            this.eventFormatter = eventFormatter;
        }

        public bool IsRequired()
        {
            return CurrentVersion > GetCurrentVersion();
        }

        public async Task UpgradeAsync(IUpgradeContext context)
        {
            int currentVersion = GetCurrentVersion();

            if (currentVersion < 1)
                await UpgradeVersion1();

            if (currentVersion < 3)
                await UpgradeVersion2();

            ApplicationDataContainer migrationContainer = GetMigrationContainer();
            migrationContainer.Values["Version"] = CurrentVersion;
        }

        private ApplicationDataContainer GetMigrationContainer()
        {
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;

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
            EventSourcingContext();
            await RecreateReadModelContext();

            await domainFacade.CreateCategoryAsync("Home", Colors.SandyBrown);
            await domainFacade.CreateCategoryAsync("Food", Colors.OrangeRed);
            await domainFacade.CreateCategoryAsync("Eating Out", Colors.DarkRed);
        }

        private Task UpgradeVersion2()
        {
            return RecreateReadModelContext();
        }

        private void EventSourcingContext()
        {
            using (var eventSourcing = new EventSourcingContext())
            {
                eventSourcing.Database.EnsureDeleted();
                eventSourcing.Database.EnsureCreated();
                eventSourcing.Database.Migrate();
            }
        }

        private Task RecreateReadModelContext()
        {
            using (var readModels = new ReadModelContext())
            {
                readModels.Database.EnsureDeleted();
                readModels.Database.EnsureCreated();
            }

            // Should match with ReadModels.
            Rebuilder rebuilder = new Rebuilder(eventStore, eventFormatter);
            rebuilder.AddAll(new CategoryBuilder());
            rebuilder.AddAll(new OutcomeBuilder(domainFacade.PriceFactory));
            return rebuilder.RunAsync();
        }
    }
}
