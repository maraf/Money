using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo;
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

[Route("/outcomes/{Year:int}/{Month:int}")]
[Route("/overview/{Year:int}/{Month:int}")]
[Route("/overview/{Year:int}/{Month:int}/{CategoryGuid:guid}")]
[Route("/{Year:int}/{Month:int}/overview")]
[Route("/{Year:int}/{Month:int}/overview/{CategoryGuid:guid}")]
[Authorize]
public partial class OverviewMonth(
    IEventHandlerCollection EventHandlers,
    IQueryDispatcher Queries,
    Interop Interop,
    Navigator Navigator,
    ILog<Overview<MonthModel>> Log
) : Overview<MonthModel>(
    EventHandlers,
    Queries,
    Interop,
    Navigator,
    Log,
    "List of each single expense in selected month"
)
{
    [Parameter]
    public int? Year { get; set; }

    [Parameter]
    public int? Month { get; set; }

    [Parameter]
    public Guid? CategoryGuid { get; set; }

    protected override void ClearPreviousParameters()
    {
        Year = null;
        Month = null;
    }

    protected override MonthModel CreateSelectedItemFromParameters()
        => new MonthModel(Year.Value, Month.Value);

    protected override IKey CreateSelectedCategoryFromParameters()
        => CategoryGuid != null ? GuidKey.Create(CategoryGuid.Value, KeyFactory.Empty(typeof(Category)).Type) : KeyFactory.Empty(typeof(Category));

    protected override IQuery<List<OutcomeOverviewModel>> CreateItemsQuery(int pageIndex)
        => ListMonthOutcomeFromCategory.Version2(CategoryKey, SelectedPeriod, SortDescriptor, pageIndex);

    protected override bool IsContained(DateTime when)
        => SelectedPeriod == when;

    protected override string ListIncomeUrl()
    {
        if (CategoryKey.IsEmpty)
            return Navigator.UrlOverviewIncomes(SelectedPeriod);

        return null;
    }

    protected override string ChecklistUrl()
    {
        if (CategoryKey.IsEmpty)
            return Navigator.UrlChecklist(SelectedPeriod);

        return null;
    }

    protected override (string title, string url)? TrendsTitleUrl()
    {
        if (CategoryKey.IsEmpty)
            return null;

        return ("Month trends", Navigator.UrlTrends(new YearModel(SelectedPeriod.Year), CategoryKey));
    }

    protected override void OpenNextPeriod() 
        => Navigator.OpenOverview(SelectedPeriod + 1);

    protected override void OpenPrevPeriod()
        => Navigator.OpenOverview(SelectedPeriod - 1);
}
