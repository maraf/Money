﻿using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public partial class ExchangeRateCreate(ILog<ExchangeRateCreate> Log, ICommandDispatcher Commands, IQueryDispatcher Queries)
{
    protected List<string> ErrorMessages { get; } = new List<string>();

    protected List<CurrencyModel> Currencies { get; private set; }

    protected double Rate { get; set; }

    protected string SourceCurrency { get; set; }

    [Parameter]
    public string TargetCurrency { get; set; }

    [Parameter]
    public Action OnAdded { get; set; }

    protected DateTime ValidFrom { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (SourceCurrency == TargetCurrency)
            SourceCurrency = null;
    }

    public override void Show()
    {
        Reload();
        base.Show();
    }

    private async void Reload()
    {
        Reset();
        Currencies = await Queries.QueryAsync(new ListAllCurrency());
        StateHasChanged();
    }

    protected void Reset()
    {
        Rate = 1;
        ValidFrom = AppDateTime.Today;
    }

    protected void OnSaveClick()
    {
        if (Validate())
        {
            Execute();
            Modal.Hide();
        }
    }

    protected async void Execute()
    {
        await Commands.HandleAsync(new SetExchangeRate(SourceCurrency, TargetCurrency, ValidFrom, Rate));
        OnAdded?.Invoke();
        Reset();
    }

    private bool Validate()
    {
        Log.Debug($"Rate: '{Rate}', SourceCurrency: '{SourceCurrency}', TargetCurrency: '{TargetCurrency}', Valid '{ValidFrom}'");

        ErrorMessages.Clear();
        Validator.AddExchangeRate(ErrorMessages, Rate);
        Validator.AddExchangeRateSourceCurrency(ErrorMessages, SourceCurrency);
        Validator.AddExchangeRateTargetCurrency(ErrorMessages, TargetCurrency);
        Validator.AddExchangeRateCurrency(ErrorMessages, SourceCurrency, TargetCurrency);

        return ErrorMessages.Count == 0;
    }
}
