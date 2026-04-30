using Microsoft.AspNetCore.Components;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Pages;
using Money.Queries;
using Neptuo.Events;
using Neptuo.Events.Handlers;
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

namespace Money.Components;

public partial class AmountBox(ILog<AmountBox> Log, IQueryDispatcher Queries, IEventHandlerCollection EventHandlers) :
    IDisposable,
    IEventHandler<CurrencyCreated>,
    IEventHandler<CurrencyDeleted>,
    IEventHandler<CurrencyDefaultChanged>
{
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

    protected override void OnInitialized()
    {
        base.OnInitialized();

        EventHandlers
            .Add<CurrencyCreated>(this)
            .Add<CurrencyDeleted>(this)
            .Add<CurrencyDefaultChanged>(this);
    }

    public void Dispose()
    {
        EventHandlers
            .Remove<CurrencyCreated>(this)
            .Remove<CurrencyDeleted>(this)
            .Remove<CurrencyDefaultChanged>(this);
    }

    protected async void OnAuthenticationChanged(bool isAuthenticated)
    {
        if (isAuthenticated)
        {
            DecimalDigits = await Queries.QueryAsync(new GetPriceDecimalDigitsProperty());
            Currencies = await Queries.QueryAsync(new ListAllCurrency());
            defaultCurrency = await Queries.QueryAsync(new FindCurrencyDefault());
            if (Value == null)
                Currency = defaultCurrency;
            StateHasChanged();
        }
    }

    protected override void OnParametersSet()
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
        Price priceValue = null;
        if (Decimal.TryParse(rawValue, out var value))
        {
            Amount = value;
            priceValue = new Price(value, Currency);
        }

        ValueChanged?.Invoke(Value = priceValue);
    }

    protected void OnCurrencyChanged(CurrencyModel currency)
    {
        if (Currency != currency.UniqueCode)
        {
            Currency = currency.UniqueCode;
            ValueChanged?.Invoke(Value = new Price(Amount, Currency));
        }
    }

    Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
    {
        Currencies ??= new List<CurrencyModel>();
        Currencies.Add(new CurrencyModel(payload.UniqueCode, payload.Symbol, false));
        StateHasChanged();
        return Task.CompletedTask;
    }

    Task IEventHandler<CurrencyDeleted>.HandleAsync(CurrencyDeleted payload)
    {
        if (Currencies != null)
        {
            var model = Currencies.FirstOrDefault(c => c.UniqueCode == payload.UniqueCode);
            if (model != null)
                Currencies.Remove(model);

            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload)
    {
        defaultCurrency = payload.UniqueCode;

        if (Value == null)
        {
            Currency = defaultCurrency;
            StateHasChanged();
        }

        return Task.CompletedTask;
    }
}
