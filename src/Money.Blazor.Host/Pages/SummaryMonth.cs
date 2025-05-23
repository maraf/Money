﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Events;
using Neptuo.Logging;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages;

[Route("/")]
[Route("/{Year:int}/{Month:int}")]
[Authorize]
public class SummaryMonth(
    IEventHandlerCollection EventHandlers,
    IQueryDispatcher Queries,
    ILog<Summary<MonthModel>> Log,
    Navigator Navigator
) : Summary<MonthModel>(
    EventHandlers,
    Queries,
    Log,
    Navigator,
    "Per-month summary of expenses in categories"
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

    protected override (MonthModel, IReadOnlyCollection<MonthModel>) CreateSelectedPeriodFromParameters()
    {
        Log.Debug($"CreateSelectedItemFromParameters(Year='{Year}', Month='{Month}')");

        MonthModel period;
        if (Year != null && Month != null)
            period = new MonthModel(Year.Value, Month.Value);
        else
            period = AppDateTime.Now;

        return (period, new[] { period - 1, period - 2 });
    }

    protected override IQuery<List<MonthModel>> CreatePeriodsQuery()
        => new ListMonthWithExpenseOrIncome();

    protected override IQuery<Price> CreateIncomeTotalQuery(MonthModel item)
        => new GetTotalMonthIncome(item);

    protected override IQuery<Price> CreateExistingExpenseTotalQuery(MonthModel item)
        => new GetTotalMonthOutcome(item);

    protected override IQuery<Price> CreateExpectedExpenseTotalQuery(MonthModel item)
        => new GetMonthExpectedExpenseTotal(item);

    protected override IQuery<List<CategoryWithAmountModel>> CreateCategoriesQuery(MonthModel item)
        => ListMonthCategoryWithOutcome.Version2(item);

    protected override bool IsContained(DateTime changed)
        => Periods.Contains(changed);

    protected override string UrlSummary(MonthModel item)
        => Navigator.UrlSummary(item);

    protected override void OpenOverview(MonthModel item)
        => Navigator.OpenOverview(item);

    protected override void OpenOverview(MonthModel item, IKey categorykey)
        => Navigator.OpenOverview(item, categorykey);

    protected override void OpenOverviewIncomes(MonthModel item)
        => Navigator.OpenOverviewIncomes(item);

    protected override void OpenPrevPeriod()
        => Navigator.OpenSummary(SelectedPeriod - 1);

    protected override void OpenNextPeriod()
        => Navigator.OpenSummary(SelectedPeriod + 1);
}
