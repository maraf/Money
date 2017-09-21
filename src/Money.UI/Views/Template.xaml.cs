using Microsoft.EntityFrameworkCore;
using Money.Data;
using Money.Services;
using Money.ViewModels;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Controls;
using Neptuo;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
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
    public sealed partial class Template : Page, ITemplate
    {
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IFactory<IReadOnlyList<MenuItemViewModel>, bool> mainMenuFactory = ServiceProvider.MainMenuFactory;
        private readonly IReadOnlyList<MenuItemViewModel> menuItems;

        public Frame ContentFrame
        {
            get { return frmContent; }
        }

        public MainMenuListView MainMenu
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

            menuItems = mainMenuFactory.Create(true);
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
            MenuItemViewModel item = (MenuItemViewModel)((MainMenuListView)sender).ItemFromContainer(e);

            navigator
                .Open(item.Parameter)
                .Show();
        }

        public void UpdateActiveMenuItem(object parameter) 
            => UpdateActiveMenuItem(vm => mnuMain.SelectedItem = vm, menuItems, parameter);

        public void ShowLoading() => loaContent.IsActive = true;
        public void HideLoading() => loaContent.IsActive = false;

        event PointerEventHandler ITemplate.PointerPressed
        {
            add { PointerPressed += value; }
            remove { PointerPressed -= value; }
        }

        #region Common extensions
        
        public static void UpdateActiveMenuItem(Action<MenuItemViewModel> selector, IEnumerable<MenuItemViewModel> menuItems, object parameter)
        {
            Ensure.NotNull(parameter, "parameter");
            foreach (MenuItemViewModel item in menuItems)
            {
                if (item.Parameter.Equals(parameter))
                {
                    selector(item);
                    return;
                }
            }

            Type parameterType = parameter.GetType();
            foreach (MenuItemViewModel item in menuItems)
            {
                if (item.Parameter.GetType() == parameterType)
                {
                    selector(item);
                    return;
                }
            }

            selector(null);
        }

        #endregion
    }
}
