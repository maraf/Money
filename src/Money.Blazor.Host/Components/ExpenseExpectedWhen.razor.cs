﻿using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public partial class ExpenseExpectedWhen(ICommandDispatcher Commands)
{
    private DateTime originalWhen;

    [Parameter]
    public IKey ExpenseKey { get; set; }

    [Parameter]
    public DateTime When { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        originalWhen = When;
    }

    protected void OnSaveClick()
    {
        if (originalWhen != When)
        {
            Execute();
            OnParametersSet();
            Modal.Hide();
        }
    }

    private async void Execute()
        => await Commands.HandleAsync(new ChangeExpenseExpectedWhen(ExpenseKey, When));
}
