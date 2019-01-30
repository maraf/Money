using Neptuo;
using Neptuo.Activators;
using Neptuo.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Money.Bootstrap
{
    internal abstract class UpgradeServiceBase : IUpgradeService
    {
        private readonly IFactory<ApplicationDataContainer> storageContainerFactory;
        private readonly int currentVersion;
        private readonly List<Func<Task>> completed = new List<Func<Task>>();

        public event Func<Task> Completed
        {
            add => completed.Add(value);
            remove => completed.Remove(value);
        }

        public UpgradeServiceBase(IFactory<ApplicationDataContainer> storageContainerFactory, int currentVersion)
        {
            Ensure.NotNull(storageContainerFactory, "storageContainerFactory");
            Ensure.Positive(currentVersion, "currentVersion");
            this.storageContainerFactory = storageContainerFactory;
            this.currentVersion = currentVersion;
        }

        public bool IsRequired()
        {
            return this.currentVersion > GetCurrentVersion();
        }

        public async Task UpgradeAsync(IUpgradeContext context)
        {
            int currentVersion = GetCurrentVersion();
            if (this.currentVersion <= currentVersion)
                return;

            if (currentVersion == 0)
            {
                context.TotalSteps(1);
                await DelayAsync();

                await FirstRunAsync(context, currentVersion);
            }
            else
            {
                context.TotalSteps(this.currentVersion - currentVersion);
                await DelayAsync();

                await UpgradeOverrideAsync(context, currentVersion);
            }

            SetCurrentVersion(this.currentVersion);

            foreach (var item in completed)
                await item();

            await DelayAsync();
        }

        private static Task DelayAsync()
            => Task.Delay(300);

        protected abstract Task FirstRunAsync(IUpgradeContext context, int currentVersion);
        protected abstract Task UpgradeOverrideAsync(IUpgradeContext context, int currentVersion);

        private ApplicationDataContainer GetMigrationContainer()
        {
            ApplicationDataContainer root = storageContainerFactory.Create();

            ApplicationDataContainer migrationContainer;
            if (!root.Containers.TryGetValue("Migration", out migrationContainer))
                migrationContainer = root.CreateContainer("Migration", ApplicationDataCreateDisposition.Always);

            return migrationContainer;
        }

        public int GetCurrentVersion()
        {
            ApplicationDataContainer migrationContainer = GetMigrationContainer();
            int currentVersion = (int?)migrationContainer.Values["Version"] ?? 0;
            return currentVersion;
        }

        public void SetCurrentVersion(int currentVersion)
        {
            ApplicationDataContainer migrationContainer = GetMigrationContainer();
            migrationContainer.Values["Version"] = currentVersion;
        }
    }
}
