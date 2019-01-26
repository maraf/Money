using Money.Bootstrap;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Money.Services
{
    internal class AppDataService
    {
        private readonly StorageFactory storageFactory;
        private readonly UpgradeServiceBase upgradeService;

        internal AppDataService(StorageFactory storageFactory, UpgradeServiceBase upgradeService)
        {
            Ensure.NotNull(storageFactory, "storageFactory");
            Ensure.NotNull(upgradeService, "upgradeService");
            this.storageFactory = storageFactory;
            this.upgradeService = upgradeService;
        }

        private async Task AddFileToZipAsync(ZipArchive archive, string relativePath)
        {
            StorageFile eventSourcing = await ApplicationData.Current.LocalFolder.GetFileAsync(relativePath);
            archive.CreateEntryFromFile(eventSourcing.Path, eventSourcing.Name);
        }

        public async Task ExportAsync(Stream targetContent)
        {
            using (ZipArchive archive = new ZipArchive(targetContent, ZipArchiveMode.Create))
            {
                await AddFileToZipAsync(archive, storageFactory.GetEventSourcingFilePath());
                await AddFileToZipAsync(archive, storageFactory.GetReadModelFilePath());

                ZipArchiveEntry migrations = archive.CreateEntry("Migration.txt");
                using (StreamWriter migrationsWriter = new StreamWriter(migrations.Open()))
                    migrationsWriter.Write(upgradeService.GetCurrentVersion());
            }
        }

        public async Task ImportAsync(Stream sourceContent)
        {

        }
    }
}
