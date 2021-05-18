using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    [Route("/outcomes/{Year:int}/{Month:int}")]
    [Route("/overview/{Year:int}/{Month:int}")]
    [Route("/overview/{Year:int}/{Month:int}/{CategoryGuid:guid}")]
    [Route("/{Year:int}/{Month:int}/overview")]
    [Route("/{Year:int}/{Month:int}/overview/{CategoryGuid:guid}")]
    [Authorize]
    public class OverviewMonth : Overview<MonthModel>
    {
        [Parameter]
        public int Year { get; set; }

        [Parameter]
        public int Month { get; set; }

        [Parameter]
        public Guid? CategoryGuid { get; set; }

        public OverviewMonth()
            => SubTitle = "List of each single expense in selected month";

        protected override MonthModel CreateSelectedItemFromParameters()
            => new MonthModel(Year, Month);

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

        protected override (string title, string url)? TrendsTitleUrl()
        {
            if (CategoryKey.IsEmpty)
                return null;

            return ("Month trends", Navigator.UrlTrends(new YearModel(SelectedPeriod.Year), CategoryKey));
        }
    }
}
