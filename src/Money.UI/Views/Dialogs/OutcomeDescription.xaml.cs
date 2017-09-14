using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views.Dialogs
{
    public sealed partial class OutcomeDescription : ContentDialog
    {
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(string),
            typeof(OutcomeDescription),
            new PropertyMetadata(null)
        );

        public bool IsEnterPressed { get; private set; }

        public OutcomeDescription()
        {
            InitializeComponent();
        }

        private void tbxDescription_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                IsEnterPressed = true;
                Hide();
            }
            else if (e.Key == VirtualKey.Escape)
            {
                if (sender == tbxDescription && tbxDescription.Text != string.Empty)
                {
                    tbxDescription.Text = string.Empty;
                    e.Handled = true;
                }
            }
        }

        private void tbxDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            tbxDescription.SelectAll();
        }
    }
}
