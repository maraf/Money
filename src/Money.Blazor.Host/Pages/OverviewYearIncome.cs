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

[Route("/{Year:int}/overview/incomes")]
[Authorize]
public partial class OverviewYearIncome(
    IEventHandlerCollection EventHandlers,
    IQueryDispatcher Queries,
    Interop Interop,
    Navigator Navigator
) : OverviewIncome<YearModel>(
    EventHandlers,
    Queries,
    Interop,
    Navigator,
    "List of each single income in selected year"
)
{
    [Parameter]
    public int? Year { get; set; }

    protected override void ClearPreviousParameters()
    {
        Year = null;
    }

    protected override YearModel CreateSelectedItemFromParameters()
        => new YearModel(Year.Value);

    protected override IReadOnlyCollection<YearModel> CreatePeriodGuesses()
        => new YearModel[] { SelectedPeriod - 1, SelectedPeriod - 2 };

    protected override IQuery<List<YearModel>> CreatePeriodsQuery()
        => new ListYearWithExpenseOrIncome();

    protected override string UrlOverviewIncomes(YearModel period)
        => Navigator.UrlOverviewIncomes(period);

    protected override IQuery<List<IncomeOverviewModel>> CreateItemsQuery(int pageIndex)
        => new ListYearIncome(SelectedPeriod, SortDescriptor, pageIndex);

    protected override bool IsContained(DateTime when)
        => SelectedPeriod == when;

    protected override string ListExpenseUrl()
        => Navigator.UrlOverview(SelectedPeriod);

    protected override void OpenNextPeriod()
        => Navigator.OpenOverviewIncomes(SelectedPeriod + 1);

    protected override void OpenPrevPeriod()
        => Navigator.OpenOverviewIncomes(SelectedPeriod - 1);
}
