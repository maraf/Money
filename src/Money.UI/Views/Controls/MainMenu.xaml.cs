using Money.ViewModels;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
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
    public sealed partial class MainMenu : UserControl
    {
        private readonly List<MenuItemViewModel> items;

        public event EventHandler<MainMenuEventArgs> ItemSelected;

        public IReadOnlyList<MenuItemViewModel> Items => items;

        public MenuItemViewModel SelectedItem
        {
            get => mlvList.SelectedItem as MenuItemViewModel;
            set => mlvList.SelectedItem = value;
        }

        public MainMenu()
        {
            InitializeComponent();

            items = new List<MenuItemViewModel>()
            {
                new MenuItemViewModel("Month", "\uE908", new SummaryParameter()) { Group = "Summary" },
                new MenuItemViewModel("Create Outcome", "\uE108", new OutcomeParameter()) { Group = "Summary" },

                new MenuItemViewModel("Categories", "\uE8FD", new CategoryListParameter()) { Group = "Manage" },
                new MenuItemViewModel("Currencies", "\uE1D0", new CurrencyParameter()) { Group = "Manage" },

                new MenuItemViewModel("Settings", "\uE713", new AboutParameter()) { Group = "Settings" },
            };

            MenuItemsSource.Source = items.GroupBy(i => i.Group);

            // TODO: Remove after making the synchronization of selected item.
            mlvList.SelectedIndex = 0;
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
