using Microsoft.EntityFrameworkCore;
using Money.Data;
using Money.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views.Controls
{
    public sealed partial class DevelopmentTools : StackPanel
    {
        private readonly IDevelopmentService developmentService = ServiceProvider.DevelopmentService;

        public DevelopmentTools()
        {
            InitializeComponent();

#if DEBUG
            Visibility = Visibility.Visible;
#else
            Visibility = Visibility.Collapsed;
#endif
        }

        private async Task ShowExitDialogAsync()
        {
            MessageDialog dialog = new MessageDialog("Do you want to exit the application?");
            UICommand yes = new UICommand("Yes");
            dialog.Commands.Add(yes);
            dialog.Commands.Add(new UICommand("No"));

            if (await dialog.ShowAsync() == yes)
                CoreApplication.Exit();
        }

        private async void btnClearStorage_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            using (var readModels = new ReadModelContext())
            {
                readModels.Database.EnsureDeleted();
                readModels.Database.EnsureCreated();
            }

            using (var eventSourcing = new EventSourcingContext())
            {
                eventSourcing.Database.EnsureDeleted();
                eventSourcing.Database.EnsureCreated();
                await eventSourcing.Database.MigrateAsync();
            }

            foreach (string containerName in ApplicationData.Current.LocalSettings.Containers.Select(c => c.Key))
                ApplicationData.Current.LocalSettings.DeleteContainer(containerName);

            await ShowExitDialogAsync();
#endif
        }

        private async void btnUploadStorage_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".db");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.ReplaceExisting);
                await ShowExitDialogAsync();
            }
#endif
        }

        private async void btnDownloadStorage_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            foreach (StorageFile source in await ApplicationData.Current.LocalFolder.GetFilesAsync())
            {
                FileSavePicker picker = new FileSavePicker();
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.FileTypeChoices.Add(source.DisplayName, new List<string>() { ".db" });
                picker.SuggestedFileName = source.Name;

                StorageFile target = await picker.PickSaveFileAsync();
                if (target != null)
                {
                    using (Stream sourceContent = await source.OpenStreamForReadAsync())
                    using (Stream targetContent = await target.OpenStreamForWriteAsync())
                        sourceContent.CopyTo(targetContent);
                }
            }
#endif
        }

        private async void btnSetRevisionStorage_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Set storage revision";

            TextBox input = new TextBox();
            dialog.Content = input;

            dialog.PrimaryButtonText = "Ok";
            dialog.SecondaryButtonText = "Cancel";

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (Int32.TryParse(input.Text, out int revision))
                {
                    ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
                    ApplicationDataContainer migrationContainer;
                    if (root.Containers.TryGetValue("Migration", out migrationContainer))
                        migrationContainer.Values["Version"] = revision;

                }
            }

            await ShowExitDialogAsync();
#endif
        }

        private async void btnRebuildReadModels_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            await developmentService.RebuildReadModelsAsync();
            await ShowExitDialogAsync();
#endif
        }
    }
}
