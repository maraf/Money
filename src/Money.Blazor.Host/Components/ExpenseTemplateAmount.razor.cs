﻿using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public partial class ExpenseTemplateAmount(ICommandDispatcher Commands, IQueryDispatcher Queries)
{
    private Price originalAmount;

    public List<CurrencyModel> Currencies { get; private set; }
    protected List<string> ErrorMessages { get; } = new List<string>();

    [Parameter]
    public IKey ExpenseTemplateKey { get; set; }

    [Parameter]
    public Price Amount { get; set; }

    protected async override Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        SetOriginal();
        Currencies = await Queries.QueryAsync(new ListAllCurrency());
    }

    private void SetOriginal()
    {
        originalAmount = Amount;
    }

    protected void OnSaveClick()
    {
        if (Validate() && (originalAmount != Amount))
        {
            Execute();
            SetOriginal();
            Modal.Hide();
        }
    }

    private bool Validate()
    {
        ErrorMessages.Clear();
        Validator.AddExpenseTemplateAmount(ErrorMessages, Amount);
        return ErrorMessages.Count == 0;
    }

    private async void Execute()
    {
        await Commands.HandleAsync(new ChangeExpenseTemplateAmount(ExpenseTemplateKey, Amount));
    }
}
