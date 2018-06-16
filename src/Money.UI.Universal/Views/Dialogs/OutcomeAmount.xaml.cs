using Money.Models;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Observables.Collections;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

        private string presetCurrency;

        public string Currency
        {
            get { return (cbxCurrency.SelectedItem as CurrencyModel)?.UniqueCode; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    cbxCurrency.SelectedItem = null;
                }
                else
                {
                    CurrencyModel model = Currencies.FirstOrDefault(c => c.UniqueCode == value);
                    if (model == null)
                        presetCurrency = value;
                    else
                        cbxCurrency.SelectedItem = model;
                }
            }
        }

        public ObservableCollection<CurrencyModel> Currencies { get; private set; }

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
            "ErrorMessage",
            typeof(string),
            typeof(OutcomeAmount),
            new PropertyMetadata(null, OnErrorMessageChanged)
        );

        private static void OnErrorMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OutcomeAmount control = (OutcomeAmount)d;

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
            {
                if (presetCurrency == null)
                    cbxCurrency.SelectedItem = currencies.FirstOrDefault(c => c.IsDefault);
                else
                    cbxCurrency.SelectedItem = currencies.FirstOrDefault(c => c.UniqueCode == presetCurrency);

                presetCurrency = null;
            }
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
                if (sender == tbxAmount && tbxAmount.Text != "0")
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

        public async Task<ContentDialogResult> ShowAsync(bool isSecondaryCancel)
        {
            decimal amount;
            ContentDialogResult result;
            do
            {
                result = await ShowAsync();
                if (result == ContentDialogResult.None)
                {
                    if (Result == null)
                        return ContentDialogResult.None;

                    result = Result.Value;
                }

                amount = Value;
                if (amount <= 0)
                    ErrorMessage = "Amount must be greater than zero.";
            }
            while (amount <= 0 && (!isSecondaryCancel || result != ContentDialogResult.Secondary));

            return result;
        }
    }
}
