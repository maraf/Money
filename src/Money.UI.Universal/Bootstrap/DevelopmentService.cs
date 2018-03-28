using Money.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.Profile;

namespace Money.Bootstrap
{
    internal class DevelopmentService : IDevelopmentService
    {
        private readonly UpgradeService upgradeService;
        private readonly StorageFactory storageFactory;

        internal DevelopmentService(UpgradeService upgradeService, StorageFactory storageFactory)
        {
            this.upgradeService = upgradeService;
            this.storageFactory = storageFactory;
        }

        public Task RebuildReadModelsAsync()
        {
            return upgradeService.RecreateReadModelContextAsync();
        }

        public bool IsTestDatabaseEnabled()
        {
            return storageFactory.IsTestDatabaseEnabled;
        }

        public IDevelopmentService IsTestDatabaseEnabled(bool isEnabled)
        {
            storageFactory.IsTestDatabaseEnabled = isEnabled;
            return this;
        }

        public bool IsMobileDevice()
        {
            if (ApplicationData.Current.LocalSettings.Containers.TryGetValue("DevelopmentSettings", out ApplicationDataContainer container))
            {
                if (container.Values.TryGetValue("IsMobile", out object rawValue) && rawValue is bool isEnabled && isEnabled)
                    return true;
            }

            return false;
        }

        public IDevelopmentService IsMobileDevice(bool isMobile)
        {
            ApplicationData.Current.LocalSettings
                .CreateContainer("DevelopmentSettings", ApplicationDataCreateDisposition.Always)
                .Values["IsMobile"] = isMobile;

            return this;
        }
    }
}
