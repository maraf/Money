using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Pages;
using Money.Queries;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class AmountBox
    {
        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected ILog<AmountBox> Log { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public bool AutoFocus { get; set; }

        [Parameter]
        public bool AutoSelect { get; set; }

        [Parameter]
        public Price Value { get; set; }

        [Parameter]
        public Action<Price> ValueChanged { get; set; }

        protected decimal Amount { get; set; }
        protected string Currency { get; set; }
        protected int DecimalDigits { get; set; }
        protected List<CurrencyModel> Currencies { get; private set; }

        private string defaultCurrency = null;

        protected async void OnAuthenticationChanged(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                DecimalDigits = await Queries.QueryAsync(new GetPriceDecimalDigitsProperty());
                Currencies = await Queries.QueryAsync(new ListAllCurrency());
                Currency = defaultCurrency = await Queries.QueryAsync(new FindCurrencyDefault());
                StateHasChanged();
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            base.OnParametersSet();

            Log.Debug($"Parameters '{Value?.Value}' '{Value?.Currency}'");
            if (Value != null)
            {
                Amount = Value.Value;
                Currency = Value.Currency;
            }
            else
            {
                Amount = 0;
                Currency = defaultCurrency;
            }
        }

        protected void OnValueChanged(ChangeEventArgs e)
        {
            string rawValue = e.Value?.ToString();
            if (Decimal.TryParse(rawValue, out var value))
            {
                Amount = value;
                ValueChanged?.Invoke(Value = new Price(value, Currency));
            }
        }

        protected void OnCurrencyChanged(CurrencyModel currency)
        {
            if (Currency != currency.UniqueCode)
            {
                Currency = currency.UniqueCode;
                ValueChanged?.Invoke(Value = new Price(Amount, Currency));
            }
        }
    }
}
