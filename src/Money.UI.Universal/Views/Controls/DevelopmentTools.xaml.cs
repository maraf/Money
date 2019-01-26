using Microsoft.EntityFrameworkCore;
using Money.Data;
using Money.Services;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        private readonly IDevelopmentService developmentTools = ServiceProvider.DevelopmentTools;
        private readonly IFactory<EventSourcingContext> eventSourcingContextFactory = ServiceProvider.EventSourcingContextFactory;
        private readonly IFactory<ReadModelContext> readModelContextFactory = ServiceProvider.ReadModelContextFactory;
        private readonly IFactory<ApplicationDataContainer> storageContainerFactory = ServiceProvider.StorageContainerFactory;
        private readonly RestartService restartService = ServiceProvider.RestartService;

        private bool isInitialization;

        public DevelopmentTools()
        {
            InitializeComponent();

#if DEBUG
            Visibility = Visibility.Visible;
#else
            Visibility = Visibility.Collapsed;
#endif

            try
            {
                isInitialization = true;
                tswMobile.IsOn = developmentTools.IsMobileDevice();
            }
            finally
            {
                isInitialization = false;
            }
        }

        private async Task ExecuteActionAsync(object sender, Func<Task> handler)
        {
            try
            {
                if (sender is Button button)
                    button.IsEnabled = false;

                await handler();

            }
            finally
            {
                if (sender is Button button)
                    button.IsEnabled = true;
            }
        }

        private async Task ShowExitDialogAsync()
        {
            MessageDialog dialog = new MessageDialog("Do you want to restart the application now?");
            UICommand yes = new UICommand("Yes");
            dialog.Commands.Add(yes);
            dialog.Commands.Add(new UICommand("No"));

            if (await dialog.ShowAsync() == yes)
                await restartService.RestartAsync();
        }

        private async void btnClearStorage_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteActionAsync(sender, async () =>
            {
                using (var readModels = readModelContextFactory.Create())
                {
                    readModels.Database.EnsureDeleted();
                    readModels.Database.EnsureCreated();
                }

                using (var eventSourcing = eventSourcingContextFactory.Create())
                {
                    eventSourcing.Database.EnsureDeleted();
                    eventSourcing.Database.EnsureCreated();
                    await eventSourcing.Database.MigrateAsync();
                }

                ApplicationDataContainer rootContainer = storageContainerFactory.Create();
                foreach (string containerName in rootContainer.Containers.Select(c => c.Key))
                    rootContainer.DeleteContainer(containerName);

                await ShowExitDialogAsync();
            });
        }

        private async void btnUploadStorage_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteActionAsync(sender, async () =>
            {
                FileOpenPicker picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".db");

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.ReplaceExisting);
                    await ShowExitDialogAsync();
                }
            });
        }

        private async void btnDownloadStorage_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteActionAsync(sender, async () =>
            {
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
            });
        }
        
        private async void btnSetRevisionStorage_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteActionAsync(sender, async () =>
            {
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
                        ApplicationDataContainer rootContainer = storageContainerFactory.Create();
                        ApplicationDataContainer migrationContainer;
                        if (rootContainer.Containers.TryGetValue("Migration", out migrationContainer))
                            migrationContainer.Values["Version"] = revision;

                    }
                }

                await ShowExitDialogAsync();
            });
        }

        private async void btnRebuildReadModels_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteActionAsync(sender, async () =>
            {
                await developmentTools.RebuildReadModelsAsync();
                await ShowExitDialogAsync();
            });
        }

        private async void tswMobile_Toggled(object sender, RoutedEventArgs e)
        {
            if (isInitialization)
                return;

            developmentTools.IsMobileDevice(tswMobile.IsOn);
            await ShowExitDialogAsync();
        }
    }
}
