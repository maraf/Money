using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo;
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

public partial class ExpenseTemplateRecurrence(ICommandDispatcher Commands)
{
    protected RecurrencePeriod? originalPeriod;
    protected int? originalDayInPeriod;
    protected DateTime? originalDueDate;
    protected int? originalMonthInPeriod;
    protected int? originalEveryXPeriods;

    protected List<string> ErrorMessages { get; } = new List<string>();
    protected List<CategoryModel> Categories { get; private set; }
    protected DateTime DueDateBinding { get; set; }

    [Parameter]
    public IKey ExpenseTemplateKey { get; set; }

    [Parameter]
    public RecurrencePeriod? Period { get; set; }

    [Parameter]
    public int? DayInPeriod { get; set; }

    [Parameter]
    public int? EveryXPeriods { get; set; }

    [Parameter]
    public int? MonthInPeriod { get; set; }

    [Parameter]
    public DateTime? DueDate { get; set; }

    protected async override Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        SetOriginal();
    }

    public async override Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
        
        DueDateBinding = DueDate ?? AppDateTime.Today;
        
        if (DayInPeriod == null)
            DayInPeriod = 1;
        
        if (MonthInPeriod == null)
            MonthInPeriod = 1;
        
        if (EveryXPeriods == null)
            EveryXPeriods = 2;
    }

    private void SetOriginal()
    {
        originalPeriod = Period;
        originalDayInPeriod = DayInPeriod;
        originalDueDate = DueDate;
        originalMonthInPeriod = MonthInPeriod;
        originalEveryXPeriods = EveryXPeriods;
    }

    protected void OnSaveClick()
    {
        if (Validate())
        {
            Execute();
            SetOriginal();
            Modal.Hide();
        }
    }

    private bool Validate()
    {
        ErrorMessages.Clear();
        
        if (Period == RecurrencePeriod.Monthly && DayInPeriod <= 0)
        {
            ErrorMessages.Add("Day in month must be greater than 0");
        }
        else if (Period == RecurrencePeriod.XMonths)
        {
            if (DayInPeriod <= 0)
                ErrorMessages.Add("Day in month must be greater than 0");
            
            if (EveryXPeriods <= 0)
                ErrorMessages.Add("Every X periods must be great than 0");
        }
        else if (Period == RecurrencePeriod.Yearly)
        {
            if (DayInPeriod <= 0)
                ErrorMessages.Add("Day in month must be greater than 0");
            
            if (MonthInPeriod <= 0)
                ErrorMessages.Add("Month in year must be greater than 0");
        }
        else if(Period == RecurrencePeriod.Single && DueDateBinding == DateTime.MinValue)
        {
            ErrorMessages.Add("Due date must be set");
        }

        return ErrorMessages.Count == 0;
    }

    private async void Execute()
    {
        if (Period == null)
            await Commands.HandleAsync(new ClearExpenseTemplateRecurrence(ExpenseTemplateKey));
        else if (Period == RecurrencePeriod.Monthly)
            await Commands.HandleAsync(new SetExpenseTemplateMonthlyRecurrence(ExpenseTemplateKey, DayInPeriod.Value));
        else if (Period == RecurrencePeriod.XMonths)
            await Commands.HandleAsync(new SetExpenseTemplateXMonthsRecurrence(ExpenseTemplateKey, EveryXPeriods.Value, DayInPeriod.Value));
        else if (Period == RecurrencePeriod.Yearly)
            await Commands.HandleAsync(new SetExpenseTemplateYearlyRecurrence(ExpenseTemplateKey, MonthInPeriod.Value, DayInPeriod.Value));
        else if (Period == RecurrencePeriod.Single)
            await Commands.HandleAsync(new SetExpenseTemplateSingleRecurrence(ExpenseTemplateKey, DueDateBinding));
        else
            throw Ensure.Exception.NotSupported($"The value '{Period}' is not supported.");
    }
}
