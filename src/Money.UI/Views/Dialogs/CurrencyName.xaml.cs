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
    public sealed partial class CurrencyName : ContentDialog
    {
        public string UniqueCode
        {
            get { return (string)GetValue(UniqueCodeProperty); }
            set { SetValue(UniqueCodeProperty, value); }
        }

        public static readonly DependencyProperty UniqueCodeProperty = DependencyProperty.Register(
            "UniqueCode",
            typeof(string),
            typeof(CurrencyName),
            new PropertyMetadata(null)
        );

        public string Symbol
        {
            get { return (string)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
            "Symbol",
            typeof(string),
            typeof(CurrencyName),
            new PropertyMetadata(null)
        );

        public bool IsUniqueCodeEnabled
        {
            get { return tbxUniqueCode.IsEnabled; }
            set { tbxUniqueCode.IsEnabled = value; }
        }

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
            "ErrorMessage",
            typeof(string),
            typeof(CurrencyName),
            new PropertyMetadata(null, OnErrorMessageChanged)
        );

        private static void OnErrorMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurrencyName control = (CurrencyName)d;

            if (String.IsNullOrEmpty(control.ErrorMessage))
            {
                control.tblError.Text = String.Empty;
                control.tblError.Visibility = Visibility.Collapsed;
            }
            else
            {
                control.tblError.Text = control.ErrorMessage;
                control.tblError.Visibility = Visibility.Visible;
            }
        }

        public bool IsEnterPressed { get; private set; }

        public CurrencyName()
        {
            InitializeComponent();
        }

        private void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            IsEnterPressed = false;
        }

        private void tbxUniqueCode_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                IsEnterPressed = true;
                Hide();
            }
            else if (e.Key == VirtualKey.Escape)
            {
                if (sender == tbxUniqueCode && tbxUniqueCode.Text != string.Empty)
                {
                    tbxUniqueCode.Text = string.Empty;
                    e.Handled = true;
                }
            }
        }

        private void tbxSymbol_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                IsEnterPressed = true;
                Hide();
            }
            else if (e.Key == VirtualKey.Escape)
            {
                if (sender == tbxSymbol && tbxSymbol.Text != string.Empty)
                {
                    tbxSymbol.Text = string.Empty;
                    e.Handled = true;
                }
            }
        }
    }
}
