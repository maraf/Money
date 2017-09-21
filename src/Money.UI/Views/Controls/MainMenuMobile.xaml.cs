using Money.ViewModels;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Neptuo.Activators;
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

namespace Money.Views.Controls
{
    public sealed partial class MainMenuMobile : UserControl
    {
        private readonly IFactory<IReadOnlyList<MenuItemViewModel>, bool> mainMenuFactory = ServiceProvider.MainMenuFactory;
        private readonly IReadOnlyList<MenuItemViewModel> items;

        public event EventHandler<MainMenuEventArgs> ItemSelected;

        public IReadOnlyList<MenuItemViewModel> Items => items;

        public MenuItemViewModel SelectedItem
        {
            get => mlvList.SelectedItem as MenuItemViewModel;
            set => mlvList.SelectedItem = value;
        }

        public MainMenuMobile()
        {
            InitializeComponent();

            items = mainMenuFactory.Create(false);
            MenuItemsSource.Source = items.GroupBy(i => i.Group);
        }

        private void mlvList_ItemInvoked(object sender, ListViewItem e)
        {
            MenuItemViewModel item = (MenuItemViewModel)((MainMenuListView)sender).ItemFromContainer(e);
            ItemSelected(this, new MainMenuEventArgs(item));
        }

        public void SelectItem(MenuItemViewModel item)
        {
            mlvList.SelectedItem = item;
        }
    }
}
