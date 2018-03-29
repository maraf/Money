using Money.Services.Globalization;
using Money.Models.Queries;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Controls
{
    public sealed class Currency : UserControl
    {
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private readonly ICurrencyProvider currencyProvider = ServiceProvider.CurrencyProvider;
        private ICurrency currency;

        public Price Price
        {
            get { return (Price)GetValue(PriceProperty); }
            set { SetValue(PriceProperty, value); }
        }

        public static readonly DependencyProperty PriceProperty = DependencyProperty.Register(
            "Price",
            typeof(Price),
            typeof(Currency),
            new PropertyMetadata(null, OnPriceChanged)
        );

        private static void OnPriceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Currency control = (Currency)d;
            control.OnPriceChanged();
        }
        
        private async void OnPriceChanged()
        {
            Price price = Price;
            if (price == null)
            {
                Content = null;
                return;
            }

            if (price.Currency != currency?.UniqueCode)
            {
                currency = currencyProvider.Get(price.Currency);
                string symbol = await queryDispatcher.QueryAsync(new GetCurrencySymbol(price.Currency));

                if (symbol != currency.Symbol)
                    currency = currency.ForCustomSymbol(symbol);
            }

            Content = new TextBlock()
            {
                Text = currency.Format(price.Value),
                FontSize = FontSize,
                Foreground = Foreground,
            };
        }
    }
}
