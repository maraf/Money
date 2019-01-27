using Money.Services;
using Money.UI;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views
{
    [NavigationParameter(typeof(AboutParameter))]
    public sealed partial class About : Page
    {
        private readonly IDevelopmentService developmentTools = ServiceProvider.DevelopmentTools;
        private readonly RestartService restartService = ServiceProvider.RestartService;
        private readonly AppDataService appDataService = ServiceProvider.AppDataService;
        private readonly INavigator navigator = ServiceProvider.Navigator;

        public About()
        {
            InitializeComponent();

            DatabaseSwitch.IsOn = developmentTools.IsTestDatabaseEnabled();
            DevelopmentToolsTitle.Visibility = DevelopmentTools.Visibility;
        }

        private void DatabaseSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (DatabaseSwitch.IsOn != developmentTools.IsTestDatabaseEnabled())
            {
                string state = DatabaseSwitch.IsOn ? "on" : "off";
                string message = $"Do you really want to turn test database {state}?{Environment.NewLine}{Environment.NewLine}No data will be lost and after switching back, you can continue where left off.{Environment.NewLine}{Environment.NewLine}Application needs a restart to complete the operation.";

                navigator
                    .Message(message)
                    .Button("Yes", new SwitchDatabaseCommand(developmentTools, restartService, DatabaseSwitch.IsOn))
                    .ButtonClose("No")
                    .Show();
            }
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.FileTypeChoices.Add("Zip Archive", new List<string>() { ".zip" });
            picker.SuggestedFileName = $"NeptuoMoney_AppData_{DateTime.Now.ToString("yyyy-MM-dd")}.zip";

            StorageFile target = await picker.PickSaveFileAsync();
            if (target != null)
            {
                using (Stream targetContent = await target.OpenStreamForWriteAsync())
                    await appDataService.ExportAsync(targetContent);
            }
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".zip");

            StorageFile source = await picker.PickSingleFileAsync();
            if (source != null)
            {
                using (Stream sourceContent = await source.OpenStreamForReadAsync())
                {
                    bool isSuccess = await appDataService.ImportAsync(sourceContent);
                    if (isSuccess)
                    {
                        navigator
                            .Message("Import was successful. We need to restart the application to reload data.")
                            .Button("OK", new RestartCommand(restartService))
                            .ButtonClose("Cancel")
                            .Show();
                    }
                    else
                    {
                        navigator
                            .Message("Someting went wrong and import for was not successful.")
                            .ButtonClose("OK")
                            .Show();
                    }
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            navigator
                .Message("Do you realy want to delete all your data?")
                .Button("Yes", new DeleteAllDataCommand(appDataService, restartService))
                .ButtonClose("No")
                .Show();
        }

        private class SwitchDatabaseCommand : AsyncCommand
        {
            private readonly IDevelopmentService developmentTools;
            private readonly RestartService restartService;
            private readonly bool isEnabled;

            public SwitchDatabaseCommand(IDevelopmentService developmentTools, RestartService restartService, bool isEnabled)
            {
                Ensure.NotNull(developmentTools, "developmentTools");
                Ensure.NotNull(restartService, "restartService");
                this.developmentTools = developmentTools;
                this.restartService = restartService;
                this.isEnabled = isEnabled;
            }

            protected override bool CanExecuteOverride() => true;

            protected override async Task ExecuteAsync(CancellationToken cancellationToken)
            {
                developmentTools.IsTestDatabaseEnabled(isEnabled);
                await restartService.RestartAsync();
            }
        }

        private class RestartCommand : AsyncCommand
        {
            private readonly RestartService restartService;

            public RestartCommand(RestartService restartService)
            {
                Ensure.NotNull(restartService, "restartService");
                this.restartService = restartService;
            }
            
            protected override bool CanExecuteOverride() => true;

            protected override Task ExecuteAsync(CancellationToken cancellationToken) => restartService.RestartAsync();
        }

        private class DeleteAllDataCommand : AsyncCommand
        {
            private readonly AppDataService appDataService;
            private readonly RestartService restartService;

            public DeleteAllDataCommand(AppDataService appDataService, RestartService restartService)
            {
                Ensure.NotNull(appDataService, "appDataService");
                Ensure.NotNull(restartService, "restartService");
                this.appDataService = appDataService;
                this.restartService = restartService;
            }

            protected override bool CanExecuteOverride() => true;

            protected override async Task ExecuteAsync(CancellationToken cancellationToken)
            {
                await appDataService.DeleteAllAsync();
                await restartService.RestartAsync();
            }
        }
    }
}
