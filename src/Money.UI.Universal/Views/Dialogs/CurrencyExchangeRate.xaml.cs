using Money.Models;
using Money.Models.Queries;
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
        private List<CurrencyModel> currencies;
        private bool isSourceCurrencyChanging;

        public string SourceCurrency
        {
            get { return (string)GetValue(SourceCurrencyProperty); }
            set { SetValue(SourceCurrencyProperty, value); }
        }

        public static readonly DependencyProperty SourceCurrencyProperty = DependencyProperty.Register(
            "SourceCurrency", 
            typeof(string), 
            typeof(CurrencyExchangeRate), 
            new PropertyMetadata(null, OnSourceCurrencyChanged)
        );

        private static void OnSourceCurrencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurrencyExchangeRate control = (CurrencyExchangeRate)d;
            control.OnSourceCurrencyChanged();
        }

        public string TargetCurrency
        {
            get { return (string)GetValue(TargetCurrencyProperty); }
            set { SetValue(TargetCurrencyProperty, value); }
        }

        public static readonly DependencyProperty TargetCurrencyProperty = DependencyProperty.Register(
            "TargetCurrency", 
            typeof(string), 
            typeof(CurrencyExchangeRate), 
            new PropertyMetadata(null, OnTargetCurrencyChanged)
        );

        private static void OnTargetCurrencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurrencyExchangeRate control = (CurrencyExchangeRate)d;
            control.OnTargetCurrencyChanged();
        }

        private CurrencyModel TargetCurrencyModel { get; set; }

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

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
            "ErrorMessage",
            typeof(string),
            typeof(CurrencyExchangeRate),
            new PropertyMetadata(null, OnErrorMessageChanged)
        );

        private static void OnErrorMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurrencyExchangeRate control = (CurrencyExchangeRate)d;

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

        public CurrencyExchangeRate(IQueryDispatcher queryDispatcher)
        {
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            this.queryDispatcher = queryDispatcher;

            InitializeComponent();
            Initialize();

            cbxCurrency.Focus(FocusState.Keyboard);
        }

        private async void Initialize()
        {
            currencies = await queryDispatcher.QueryAsync(new ListAllCurrency());
            OnTargetCurrencyChanged();
        }

        private void OnTargetCurrencyChanged()
        {
            List<CurrencyModel> currencies = this.currencies
                .Where(c => c.UniqueCode != TargetCurrency)
                .ToList();

            cbxCurrency.ItemsSource = currencies;

            TargetCurrencyModel = this.currencies.FirstOrDefault(c => c.UniqueCode == TargetCurrency);
            // TODO: Not working.
            //if (currencies.Count > 0)
            //    cbxCurrency.SelectedIndex = 0;
        }

        private void OnSourceCurrencyChanged()
        {
            if (isSourceCurrencyChanging)
                return;

            CurrencyModel model = currencies.FirstOrDefault(c => c.UniqueCode == SourceCurrency);
            cbxCurrency.SelectedItem = model;
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

        private void cbxCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                isSourceCurrencyChanging = true;

                if (cbxCurrency.SelectedItem is CurrencyModel model)
                    SourceCurrency = model.UniqueCode;
            }
            finally
            {
                isSourceCurrencyChanging = false;
            }
        }
    }
}
