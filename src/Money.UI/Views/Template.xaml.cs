using Money.UI;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.Controls;
using Money.Views.Navigation;
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
        public Frame ContentFrame
        {
            get { return frmContent; }
        }

        public Template()
        {
            InitializeComponent();
            
            List<MenuItemViewModel> menuItems = new List<MenuItemViewModel>()
            {
                new MenuItemViewModel("Pie Chart", "\uEB05", new EmptyParameter()) { Group = "Main" },
                new MenuItemViewModel("Summary", "\uE94C", new SummaryParameter()) { Group = "Main" },
                new MenuItemViewModel("Categories", "\uE8FD", new EmptyParameter()) { Group = "Additional" },
                new MenuItemViewModel("Currencies", "\uE1D0", new EmptyParameter()) { Group = "Additional" },
                new MenuItemViewModel("Settings", "\uE713", new EmptyParameter()) { Group = "Bottom" },
            };

            MenuItemsSource.Source = menuItems.GroupBy(i => i.Group);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.Current.Navigator
                .Open(e.Parameter)
                .Show();
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
            object paramerer = item.Parameter;
            IParameterDecorator decorator = frmContent.Content as IParameterDecorator;
            if (decorator != null)
                paramerer = decorator.Decorate(paramerer);

            navigator
                .Open(paramerer)
                .Show();
        }
    }
}
