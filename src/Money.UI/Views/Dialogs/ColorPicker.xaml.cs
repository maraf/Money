using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views.Dialogs
{
    public sealed partial class ColorPicker : ContentDialog
    {
        public Color Value
        {
            get { return (Color)GetValue(ValueProperty); }
            set
            {
                if (Value != value)
                    SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(Color),
            typeof(OutcomeAmount),
            new PropertyMetadata(Colors.Transparent)
        );

        public ColorPicker()
        {
            InitializeComponent();
        }

        private void tbxSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
            }
            else if (e.Key == VirtualKey.Escape)
            {
                if (sender == tbxSearch)
                {
                    if (tbxSearch.Text == String.Empty)
                        Hide();
                    else
                        tbxSearch.Text = String.Empty;

                    e.Handled = true;
                }
            }
        }

        private void tbxDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            tbxSearch.SelectAll();
        }
    }
}
