using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.Controls;
using Money.Views.Navigation;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

            menuItems = new List<MenuItemViewModel>()
            {
                new MenuItemViewModel("Pie Chart", "\uEB05", new SummaryParameter(SummaryViewType.PieChart)) { Group = "Summary" },
                new MenuItemViewModel("Bar Graph", "\uE94C", new SummaryParameter(SummaryViewType.BarGraph)) { Group = "Summary" },
                new MenuItemViewModel("Categories", "\uE8FD", new CategoryListParameter()) { Group = "Manage" },
                new MenuItemViewModel("Currencies", "\uE1D0", new EmptyParameter()) { Group = "Manage" },
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
    }
}
