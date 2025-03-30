using Microsoft.AspNetCore.Components;
using Money.Commands;
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

public partial class IncomeCreate(
    ILog<IncomeCreate> Log,
    ICommandDispatcher Commands, 
    IQueryDispatcher Queries,
    Navigator Navigator
)
{
    protected string Title { get; set; }
    protected string SaveButtonText { get; set; }
    protected List<string> ErrorMessages { get; } = new List<string>();

    protected List<CurrencyModel> Currencies { get; private set; }

    protected Confirm PrerequisitesConfirm { get; set; }

    [Parameter]
    public Price Amount { get; set; }

    [Parameter]
    public string Description { get; set; }

    [Parameter]
    public DateTime When { get; set; } = DateTime.UtcNow.Date;

    protected async override Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        Title = "Create a new Income";
        SaveButtonText = "Create";

        Currencies = await Queries.QueryAsync(new ListAllCurrency());
    }

    protected void OnSaveClick()
    {
        if (Validate())
        {
            Execute();
            Modal.Hide();
        }
    }

    private bool Validate()
    {
        Log.Debug($"Income: Amount: {Amount}, When: {When}.");

        ErrorMessages.Clear();
        Validator.AddIncomeAmount(ErrorMessages, Amount == null ? 0 : Amount.Value);
        Validator.AddIncomeDescription(ErrorMessages, Description);
        Validator.AddIncomeCurrency(ErrorMessages, Amount == null ? null : Amount.Currency);

        Log.Debug($"Income: Validation: {string.Join(", ", ErrorMessages)}.");
        return ErrorMessages.Count == 0;
    }

    private async void Execute()
    {
        await Commands.HandleAsync(new CreateIncome(Amount, Description, When));

        Amount = null;
        Description = null;
        StateHasChanged();
    }

    public override void Show()
    {
        if (Currencies == null || Currencies.Count == 0)
            PrerequisitesConfirm.Show();
        else
            base.Show();
    }

    protected void OnPrerequisitesConfirmed()
    {
        if (Currencies == null || Currencies.Count == 0)
            Navigator.OpenCurrencies();
    }
}
