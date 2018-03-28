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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Money.Views.Controls
{
    public sealed partial class MainMenuMobile : UserControl
    {
        private readonly IFactory<IReadOnlyList<MenuItemViewModel>, bool> mainMenuFactory = ServiceProvider.MainMenuFactory;
        private readonly IReadOnlyList<MenuItemViewModel> items;

        private BeginStoryboard showAnimation;
        private BeginStoryboard hideAnimation;

        public event EventHandler<MainMenuEventArgs> ItemSelected;

        public IReadOnlyList<MenuItemViewModel> Items => items;

        public MenuItemViewModel SelectedItem
        {
            get => mlvList.SelectedItem as MenuItemViewModel;
            set => mlvList.SelectedItem = value;
        }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(
            "IsVisible", 
            typeof(bool), 
            typeof(MainMenuMobile), 
            new PropertyMetadata(false, OnIsVisibleChanged)
        );

        private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainMenuMobile control = (MainMenuMobile)d;
            control.OnIsVisibleChanged();
        }

        public MainMenuMobile()
        {
            InitializeComponent();

            items = mainMenuFactory.Create(false);
            MenuItemsSource.Source = items.GroupBy(i => i.Group);

            showAnimation = (BeginStoryboard)Resources["MainMenuShowAnimation"];
            hideAnimation = (BeginStoryboard)Resources["MainMenuHideAnimation"];
            hideAnimation.Storyboard.Completed += OnHideAnimationCompleted;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void OnIsVisibleChanged()
        {
            if (IsVisible)
            {
                Visibility = Visibility.Visible;
                RunAnimation(showAnimation);
            }
            else
            {
                RunAnimation(hideAnimation);
            }
        }

        private void OnHideAnimationCompleted(object sender, object e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void RunAnimation(BeginStoryboard animation)
        {
            if (animation != null)
            {
                animation.Storyboard.Seek(TimeSpan.Zero);
                animation.Storyboard.Begin();
            }
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
