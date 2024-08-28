using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Queries;

namespace Money.Pages;

partial class ExpenseChecklistMonth : ComponentBase
{
    [Inject]
    protected IQueryDispatcher Queries { get; set; }

    [Inject]
    protected Navigator Navigator { get; set; }

    [Inject]
    protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }
    protected CurrencyFormatter CurrencyFormatter { get; set; }

    [Parameter]
    public int Year { get; set; }

    [Parameter]
    public int Month { get; set; }

    protected MonthModel SelectedPeriod { get; set; }
    protected List<ExpenseChecklistModel> Models { get; set; } = new();
    protected LoadingContext Loading { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        SelectedPeriod = new MonthModel(Year, Month);

        Models.Clear();
        using (Loading.Start())
            Models.AddRange(await Queries.QueryAsync(new ListMonthExpenseChecklist(SelectedPeriod)));
    }
}