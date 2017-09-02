using Money.Services.Models;
using Money.Services.Models.Queries;
using Neptuo;
using Neptuo.Observables.Collections;
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
    public sealed partial class OutcomeAmount : ContentDialog
    {
        private readonly IQueryDispatcher queryDispatcher;

        public ContentDialogResult? Result { get; set; }

        public decimal Value
        {
            get
            {
                decimal value;
                if (Decimal.TryParse(tbxAmount.Text, out value))
                    return value;

                return 0;
            }
            set
            {
                tbxAmount.Text = value.ToString();
            }
        }
        
        public string Currency
        {
            get { return (string)GetValue(CurrencyProperty); }
            set { SetValue(CurrencyProperty, value); }
        }

        public static readonly DependencyProperty CurrencyProperty = DependencyProperty.Register(
            "Currency",
            typeof(string),
            typeof(OutcomeAmount),
            new PropertyMetadata(null)
        );

        public ObservableCollection<CurrencyModel> Currencies { get; private set; }

        public OutcomeAmount(IQueryDispatcher queryDispatcher)
        {
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            this.queryDispatcher = queryDispatcher;
            Currencies = new ObservableCollection<CurrencyModel>();

            InitializeComponent();
            Initialize();
        }

        private async void Initialize()
        {
            IEnumerable<CurrencyModel> currencies = await queryDispatcher.QueryAsync(new ListAllCurrency());
            Currencies.AddRange(currencies);
            cbxCurrency.ItemsSource = Currencies;

            if (Currency == null)
                Currency = currencies.FirstOrDefault(c => c.IsDefault)?.UniqueCode;
        }

        private void tbxAmount_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Result = ContentDialogResult.Primary;
                e.Handled = true;
                Hide();
            }
            else if (e.Key == VirtualKey.Escape)
            {
                if (sender == tbxAmount)
                {
                    tbxAmount.Text = "0";
                    tbxAmount.SelectAll();
                    e.Handled = true;
                }
            }
        }

        private void tbxAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            tbxAmount.SelectAll();
        }
    }
}
