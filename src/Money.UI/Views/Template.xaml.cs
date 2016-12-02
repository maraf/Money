using Money.ViewModels;
using Money.Views.Controls;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Money.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Template : Page
    {
        public Template()
        {
            InitializeComponent();
            
            List<MenuItemViewModel> menuItems = new List<MenuItemViewModel>()
            {
                new MenuItemViewModel("Pie Chart", "\uEB05", typeof(Empty)) { Group = "Main" },
                new MenuItemViewModel("Summary", "\uE94C", typeof(GroupPage), GroupType.Month) { Group = "Main" },
                new MenuItemViewModel("Categories", "\uE8FD", typeof(Empty)) { Group = "Additional" },
                new MenuItemViewModel("Currencies", "\uE1D0", typeof(Empty)) { Group = "Additional" },
                new MenuItemViewModel("Settings", "\uE713", typeof(Empty)) { Group = "Bottom" },
            };

            MenuItemsSource.Source = menuItems.GroupBy(i => i.Group);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            frmContent.Navigate(typeof(GroupPage), GroupType.Month);
        }

        private void atbMainMenu_Checked(object sender, RoutedEventArgs e)
        {
            spvContent.IsPaneOpen = true;
        }

        private void atbMainMenu_Unchecked(object sender, RoutedEventArgs e)
        {
            spvContent.IsPaneOpen = false;
        }

        private void OnMainMenuItemInvoked(object sender, ListViewItem e)
        {
            MenuItemViewModel item = (MenuItemViewModel)((MainMenu)sender).ItemFromContainer(e);
            frmContent.Navigate(item.Page, item.Parameter);
        }
    }
}
