using Money.ViewModels;
using Money.ViewModels.Navigation;
using Money.Views.Controls;
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
    public sealed partial class TemplateMobile : Page, ITemplate
    {
        private readonly INavigator navigator = ServiceProvider.Navigator;

        public bool IsMainMenuOpened
        {
            get { return (bool)GetValue(IsMainMenuOpenedProperty); }
            set { SetValue(IsMainMenuOpenedProperty, value); }
        }

        public static readonly DependencyProperty IsMainMenuOpenedProperty = DependencyProperty.Register(
            "IsMainMenuOpened",
            typeof(bool),
            typeof(TemplateMobile),
            new PropertyMetadata(false, OnIsMainMenuOpenedChanged)
        );

        private static void OnIsMainMenuOpenedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TemplateMobile control = (TemplateMobile)d;
            if ((bool)e.NewValue)
                control.maiMenu.Visibility = Visibility.Visible;
            else
                control.maiMenu.Visibility = Visibility.Collapsed;
        }

        public Frame ContentFrame => frmContent;

        public TemplateMobile()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            navigator
                .Open(e.Parameter)
                .Show();
        }

        public void UpdateActiveMenuItem(object parameter)
        {
            Ensure.NotNull(parameter, "parameter");
            foreach (MenuItemViewModel item in maiMenu.Items)
            {
                if (item.Parameter.Equals(parameter))
                {
                    maiMenu.SelectedItem = item;
                    return;
                }
            }

            Type parameterType = parameter.GetType();
            foreach (MenuItemViewModel item in maiMenu.Items)
            {
                if (item.Parameter.GetType() == parameterType)
                {
                    maiMenu.SelectedItem = item;
                    return;
                }
            }

            maiMenu.SelectedItem = null;
        }

        public void ShowLoading()
        {
            loaContent.Visibility = Visibility.Visible;
        }

        public void HideLoading()
        {
            loaContent.Visibility = Visibility.Collapsed;
        }

        private void maiMenu_ItemSelected(object sender, MainMenuEventArgs e)
        {
            IsMainMenuOpened = false;

            navigator
                .Open(e.Item.Parameter)
                .Show();
        }
    }
}
