using Microsoft.EntityFrameworkCore;
using Money.Data;
using Money.ViewModels;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Controls;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Money.Views
{
    public sealed partial class Template : Page
    {
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly List<MenuItemViewModel> menuItems;

        public Frame ContentFrame
        {
            get { return frmContent; }
        }

        public MainMenu MainMenu
        {
            get { return mnuMain; }
        }

        public bool IsMainMenuOpened
        {
            get { return (bool)GetValue(IsMainMenuOpenedProperty); }
            set { SetValue(IsMainMenuOpenedProperty, value); }
        }

        public static readonly DependencyProperty IsMainMenuOpenedProperty = DependencyProperty.Register(
            "IsMainMenuOpened",
            typeof(bool),
            typeof(Template),
            new PropertyMetadata(false)
        );

        public Template()
        {
            InitializeComponent();

#if DEBUG
            stpDevelopment.Visibility = Visibility.Visible;
#endif

            menuItems = new List<MenuItemViewModel>()
            {
                new MenuItemViewModel("Pie Chart", "\uEB05", new SummaryParameter(SummaryViewType.PieChart)) { Group = "Summary" },
                new MenuItemViewModel("Bar Graph", "\uE94C", new SummaryParameter(SummaryViewType.BarGraph)) { Group = "Summary" },
                new MenuItemViewModel("Categories", "\uE8FD", new CategoryListParameter()) { Group = "Manage" },
                new MenuItemViewModel("Currencies", "\uE1D0", new CurrencyParameter()) { Group = "Manage" },
                new MenuItemViewModel("Settings", "\uE713", new EmptyParameter()) { Group = "Settings" },
            };

            MenuItemsSource.Source = menuItems.GroupBy(i => i.Group);

            // TODO: Remove after making the synchronization of selected item.
            mnuMain.SelectedIndex = 1;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            navigator
                .Open(e.Parameter)
                .Show();
        }

        private void OnMainMenuItemInvoked(object sender, ListViewItem e)
        {
            MenuItemViewModel item = (MenuItemViewModel)((MainMenu)sender).ItemFromContainer(e);

            navigator
                .Open(item.Parameter)
                .Show();
        }

        /// <summary>
        /// Updates currently active/selected menu item to match <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">A parameter to by selected.</param>
        public void UpdateActiveMenuItem(object parameter)
        {
            Ensure.NotNull(parameter, "parameter");
            foreach (MenuItemViewModel item in menuItems)
            {
                if (item.Parameter.Equals(parameter))
                {
                    mnuMain.SelectedItem = item;
                    return;
                }
            }

            Type parameterType = parameter.GetType();
            foreach (MenuItemViewModel item in menuItems)
            {
                if (item.Parameter.GetType() == parameterType)
                {
                    mnuMain.SelectedItem = item;
                    return;
                }
            }

            mnuMain.SelectedItem = null;
        }

        public void ShowLoading()
        {
            loaContent.IsActive = true;
        }

        public void HideLoading()
        {
            loaContent.IsActive = false;
        }

        private void btnClearStorage_Click(object sender, RoutedEventArgs e)
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
                eventSourcing.Database.Migrate();
            }

            foreach (string containerName in ApplicationData.Current.LocalSettings.Containers.Select(c => c.Key))
                ApplicationData.Current.LocalSettings.DeleteContainer(containerName);

            Application.Current.Exit();
#endif
        }

        private async void btnUploadStorage_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".db");

            StorageFile file = await picker.PickSingleFileAsync();
            await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.ReplaceExisting);

            //ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            //ApplicationDataContainer migrationContainer;
            //if (root.Containers.TryGetValue("Migration", out migrationContainer))
            //    migrationContainer.Values["Version"] = 0;

            MessageDialog dialog = new MessageDialog("Do you want to exit the application?");
            UICommand yes = new UICommand("Yes");
            dialog.Commands.Add(yes);
            dialog.Commands.Add(new UICommand("No"));

            if (await dialog.ShowAsync() == yes)
                CoreApplication.Exit();
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

                using (Stream sourceContent = await source.OpenStreamForReadAsync())
                using (Stream targetContent = await target.OpenStreamForWriteAsync())
                    sourceContent.CopyTo(targetContent);
            }
#endif
        }
    }
}
