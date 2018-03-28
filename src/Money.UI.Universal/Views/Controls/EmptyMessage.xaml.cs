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
    public sealed partial class EmptyMessage : UserControl
    {
        public string Main
        {
            get { return (string)GetValue(MainProperty); }
            set { SetValue(MainProperty, value); }
        }

        public static readonly DependencyProperty MainProperty = DependencyProperty.Register(
            "Main",
            typeof(string),
            typeof(EmptyMessage),
            new PropertyMetadata("Nothing here")
        );

        public string Additional
        {
            get { return (string)GetValue(AdditionalProperty); }
            set { SetValue(AdditionalProperty, value); }
        }

        public static readonly DependencyProperty AdditionalProperty = DependencyProperty.Register(
            "Additional",
            typeof(string),
            typeof(EmptyMessage),
            new PropertyMetadata(null)
        );

        public int ItemCount
        {
            get { return (int)GetValue(ItemCountProperty); }
            set { SetValue(ItemCountProperty, value); }
        }

        public static readonly DependencyProperty ItemCountProperty = DependencyProperty.Register(
            "ItemCount",
            typeof(int),
            typeof(EmptyMessage),
            new PropertyMetadata(0, OnItemCountChanged)
        );

        private static void OnItemCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EmptyMessage control = (EmptyMessage)d;
            control.Visibility = control.ItemCount == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public EmptyMessage()
        {
            InitializeComponent();
            Background = Resources["ApplicationPageBackgroundThemeBrush"] as Brush;
        }
    }
}
