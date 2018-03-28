using Money.Data;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Money.Bootstrap
{
    internal class StorageFactory : IFactory<EventSourcingContext>, IFactory<ReadModelContext>, IFactory<ApplicationDataContainer>
    {
        public bool IsTestDatabaseEnabled
        {
            get { return (bool?)ApplicationData.Current.LocalSettings.Values["IsTestDatabaseEnabled"] ?? false; }
            set { ApplicationData.Current.LocalSettings.Values["IsTestDatabaseEnabled"] = value; }
        }

        private string GetStorageFolderPath()
        {
            if (IsTestDatabaseEnabled)
            {
                ApplicationData.Current.LocalFolder.CreateFolderAsync("Test", CreationCollisionOption.OpenIfExists).AsTask().Wait();
                return "Test\\";
            }

            return null;
        }

        EventSourcingContext IFactory<EventSourcingContext>.Create()
        {
            return new EventSourcingContext($"Filename={GetStorageFolderPath()}EventSourcing.db");
        }

        ReadModelContext IFactory<ReadModelContext>.Create()
        {
            return new ReadModelContext($"Filename={GetStorageFolderPath()}ReadModel.db");
        }

        ApplicationDataContainer IFactory<ApplicationDataContainer>.Create()
        {
            if (IsTestDatabaseEnabled)
            {
                return ApplicationData.Current.LocalSettings
                    .CreateContainer("Test", ApplicationDataCreateDisposition.Always);
            }

            return ApplicationData.Current.LocalSettings;
        }
    }
}
