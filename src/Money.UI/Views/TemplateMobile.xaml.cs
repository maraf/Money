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
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
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
            control.maiMenu.IsVisible = (bool)e.NewValue;
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
            => Money.Views.Template.UpdateActiveMenuItem(vm => maiMenu.SelectedItem = vm, maiMenu.Items, parameter);

        public void ShowLoading() => loaContent.IsActive = true;
        public void HideLoading() => loaContent.IsActive = false;

        private void maiMenu_ItemSelected(object sender, MainMenuEventArgs e)
        {
            IsMainMenuOpened = false;

            navigator
                .Open(e.Item.Parameter)
                .Show();
        }

        event PointerEventHandler ITemplate.PointerPressed
        {
            add { PointerPressed += value; }
            remove { PointerPressed -= value; }
        }
    }
}
