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

namespace Money.Views.Controls
{
    public sealed partial class Loading : UserControl
    {
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive", 
            typeof(bool), 
            typeof(Loading), 
            new PropertyMetadata(false, OnIsActiveChanged)
        );

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Loading control = (Loading)d;
            control.Visibility = control.IsActive ? Visibility.Visible : Visibility.Collapsed;
        }

        public LoadingType Type
        {
            get { return (LoadingType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", 
            typeof(LoadingType), 
            typeof(LoadingType), 
            new PropertyMetadata(LoadingType.Ring, OnTypeChanged)
        );

        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Loading control = (Loading)d;
            switch (control.Type)
            {
                case LoadingType.Ring:
                    control.Ring.Visibility = Visibility.Visible;
                    control.Bar.Visibility = Visibility.Collapsed;
                    break;
                case LoadingType.Bar:
                    control.Bar.Visibility = Visibility.Visible;
                    control.Ring.Visibility = Visibility.Collapsed;
                    break;
                default:
                    throw Ensure.Exception.NotSupported(control.Type.ToString());
            }
        }

        public Loading()
        {
            InitializeComponent();
        }
    }
}
