using Money.Services.Models.Queries;
using Neptuo;
using Neptuo.Queries;
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
    public sealed partial class CurrencyExchangeRate : ContentDialog
    {
        private readonly IQueryDispatcher queryDispatcher;

        public string SourceCurrency
        {
            get { return (string)GetValue(SourceCurrencyProperty); }
            set { SetValue(SourceCurrencyProperty, value); }
        }

        public static readonly DependencyProperty SourceCurrencyProperty = DependencyProperty.Register(
            "SourceCurrency", 
            typeof(string), 
            typeof(CurrencyExchangeRate), 
            new PropertyMetadata(null)
        );

        public string TargetCurrency
        {
            get { return (string)GetValue(TargetCurrencyProperty); }
            set { SetValue(TargetCurrencyProperty, value); }
        }

        public static readonly DependencyProperty TargetCurrencyProperty = DependencyProperty.Register(
            "TargetCurrency", 
            typeof(string), 
            typeof(CurrencyExchangeRate), 
            new PropertyMetadata(null)
        );

        public double Rate
        {
            get { return (double)GetValue(RateProperty); }
            set { SetValue(RateProperty, value); }
        }

        public static readonly DependencyProperty RateProperty = DependencyProperty.Register(
            "Rate", 
            typeof(double), 
            typeof(CurrencyExchangeRate), 
            new PropertyMetadata(1d)
        );

        public DateTime ValidFrom
        {
            get { return (DateTime)GetValue(ValidFromProperty); }
            set { SetValue(ValidFromProperty, value); }
        }

        public static readonly DependencyProperty ValidFromProperty = DependencyProperty.Register(
            "ValidFrom", 
            typeof(DateTime), 
            typeof(CurrencyExchangeRate), 
            new PropertyMetadata(DateTime.Today)
        );

        public CurrencyExchangeRate(IQueryDispatcher queryDispatcher)
        {
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            this.queryDispatcher = queryDispatcher;

            InitializeComponent();
            Initialize();
        }

        private async void Initialize()
        {
            List<string> currencies = await queryDispatcher.QueryAsync(new ListAllCurrency());
            cbxCurrency.ItemsSource = currencies;
        }

        private void tbxRate_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
            }
            else if (e.Key == VirtualKey.Escape)
            {
                if (sender == tbxRate)
                {
                    if (tbxRate.Text == "1")
                    {
                        Hide();
                        return;
                    }

                    tbxRate.Text = "1";
                    tbxRate.SelectAll();
                    e.Handled = true;
                }
            }
        }

    }
}
