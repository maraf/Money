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
        private readonly INavigator navigator = ServiceProvider.Navigator;

        public Frame ContentFrame
        {
            get { return frmContent; }
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
            
            List<MenuItemViewModel> menuItems = new List<MenuItemViewModel>()
            {
                new MenuItemViewModel("Pie Chart", "\uEB05", new SummaryParameter(SummaryViewType.PieChart)) { Group = "Summary" },
                new MenuItemViewModel("Bar Graph", "\uE94C", new SummaryParameter(SummaryViewType.BarGraph)) { Group = "Summary" },
                new MenuItemViewModel("Categories", "\uE8FD", new EmptyParameter()) { Group = "Manage" },
                new MenuItemViewModel("Currencies", "\uE1D0", new EmptyParameter()) { Group = "Manage" },
                new MenuItemViewModel("Settings", "\uE713", new EmptyParameter()) { Group = "Settings" },
            };

            MenuItemsSource.Source = menuItems.GroupBy(i => i.Group);
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
