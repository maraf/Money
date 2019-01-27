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
        private const string MigrationFilePath = "Migration.txt";

        private readonly StorageFactory storageFactory;
        private readonly UpgradeServiceBase upgradeService;

        internal AppDataService(StorageFactory storageFactory, UpgradeServiceBase upgradeService)
        {
            Ensure.NotNull(storageFactory, "storageFactory");
            Ensure.NotNull(upgradeService, "upgradeService");
            this.storageFactory = storageFactory;
            this.upgradeService = upgradeService;
        }

        private static async Task AddFileToZipAsync(ZipArchive archive, string relativePath)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(relativePath);
            archive.CreateEntryFromFile(file.Path, file.Name);
        }

        public async Task ExportAsync(Stream targetContent)
        {
            using (ZipArchive archive = new ZipArchive(targetContent, ZipArchiveMode.Create))
            {
                await AddFileToZipAsync(archive, storageFactory.GetEventSourcingFilePath());
                await AddFileToZipAsync(archive, storageFactory.GetReadModelFilePath());

                ZipArchiveEntry migrations = archive.CreateEntry(MigrationFilePath);
                using (StreamWriter migrationsWriter = new StreamWriter(migrations.Open()))
                    migrationsWriter.Write(upgradeService.GetCurrentVersion());
            }
        }

        private static async Task ExtractFileFromZipASync(string relativePath, ZipArchiveEntry entry)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(relativePath);
            using (Stream fileContent = await file.OpenStreamForWriteAsync())
            using (Stream entryContent = entry.Open())
                await entryContent.CopyToAsync(fileContent);
        }

        private static async Task<int?> FindMigrationAsync(ZipArchiveEntry entry)
        {
            using (StreamReader reader = new StreamReader(entry.Open()))
            {
                string content = await reader.ReadToEndAsync();
                if (Int32.TryParse(content, out int version))
                    return version;
            }

            return null;
        }

        public async Task<bool> ImportAsync(Stream sourceContent)
        {
            string eventSourcingPath = storageFactory.GetEventSourcingFilePath();
            string eventSourcingFileName = Path.GetFileName(eventSourcingPath);
            string readModelPath = storageFactory.GetReadModelFilePath();
            string readModelFileName = Path.GetFileName(readModelPath);

            using (ZipArchive archive = new ZipArchive(sourceContent, ZipArchiveMode.Read))
            {
                if (archive.Entries.Count == 3)
                {
                    ZipArchiveEntry eventSourcing = archive.Entries.FirstOrDefault(e => e.FullName == eventSourcingFileName);
                    ZipArchiveEntry readModel = archive.Entries.FirstOrDefault(e => e.FullName == readModelFileName);
                    ZipArchiveEntry migration = archive.Entries.FirstOrDefault(e => e.FullName == MigrationFilePath);

                    if (eventSourcing != null && readModel != null && migration != null)
                    {
                        int? migrationVersion = await FindMigrationAsync(migration);
                        if (migrationVersion != null)
                        {
                            upgradeService.SetCurrentVersion(migrationVersion.Value);
                            await ExtractFileFromZipASync(eventSourcingPath, eventSourcing);
                            await ExtractFileFromZipASync(readModelPath, readModel);

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public async Task DeleteAllAsync()
        {
            await DeleteFileAsync(storageFactory.GetEventSourcingFilePath());
            await DeleteFileAsync(storageFactory.GetReadModelFilePath());
            upgradeService.SetCurrentVersion(0);
        }

        private static async Task DeleteFileAsync(string relativePath)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(relativePath);
            await file.DeleteAsync();
        }
    }
}
