using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Events;
using Neptuo.Queries;
using System;
using System.Collections.Generic;

namespace Money.Pages;

[Route("/{Year:int}/{Month:int}/overview/incomes")]
[Authorize]
public partial class OverviewMonthIncome(
    IEventHandlerCollection EventHandlers,
    IQueryDispatcher Queries,
    Interop Interop,
    Navigator Navigator
) : OverviewIncome<MonthModel>(
    EventHandlers,
    Queries,
    Interop,
    Navigator,
    "List of each single income in selected month"
)
{
    [Parameter]
    public int? Year { get; set; }

    [Parameter]
    public int? Month { get; set; }

    protected override void ClearPreviousParameters()
    {
        Year = null;
        Month = null;
    }

    protected override MonthModel CreateSelectedItemFromParameters()
        => new MonthModel(Year.Value, Month.Value);

    protected override IReadOnlyCollection<MonthModel> CreatePeriodGuesses()
        => new MonthModel[] { SelectedPeriod - 1, SelectedPeriod - 2 };

    protected override IQuery<List<MonthModel>> CreatePeriodsQuery()
        => new ListMonthWithExpenseOrIncome();

    protected override string UrlOverviewIncomes(MonthModel period)
        => Navigator.UrlOverviewIncomes(period);

    protected override IQuery<List<IncomeOverviewModel>> CreateItemsQuery(int pageIndex)
        => new ListMonthIncome(SelectedPeriod, SortDescriptor, pageIndex);

    protected override bool IsContained(DateTime when)
        => SelectedPeriod == when;

    protected override string ListExpenseUrl()
        => Navigator.UrlOverview(SelectedPeriod);

    protected override string ChecklistUrl()
        => Navigator.UrlChecklist(SelectedPeriod);

    protected override void OpenNextPeriod()
        => Navigator.OpenOverviewIncomes(SelectedPeriod + 1);

    protected override void OpenPrevPeriod()
        => Navigator.OpenOverviewIncomes(SelectedPeriod - 1);
}
