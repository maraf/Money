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
    [Route("/{Year:int}/overview")]
    [Route("/{Year:int}/overview/{CategoryGuid:guid}")]
    [Authorize]
    public class OverviewYear : Overview<YearModel>
    {
        [Parameter]
        public int Year { get; set; }

        [Parameter]
        public Guid? CategoryGuid { get; set; }

        public OverviewYear() 
            => SubTitle = "List of each single expense in selected year";

        protected override YearModel CreateSelectedItemFromParameters()
            => new YearModel(Year);

        protected override IKey CreateSelectedCategoryFromParameters()
            => CategoryGuid != null ? GuidKey.Create(CategoryGuid.Value, KeyFactory.Empty(typeof(Category)).Type) : KeyFactory.Empty(typeof(Category));

        protected override IQuery<List<OutcomeOverviewModel>> CreateItemsQuery(int pageIndex)
            => ListYearOutcomeFromCategory.Version2(CategoryKey, SelectedPeriod, SortDescriptor, pageIndex);

        protected override bool IsContained(DateTime when)
            => SelectedPeriod == when;

        protected override string TrendsSelectedPeriodUrl()
        {
            if (CategoryKey.IsEmpty)
                return null;

            return Navigator.UrlTrends(SelectedPeriod, CategoryKey);
        }

        protected override (string title, string url)? TrendsTitleUrl()
        {
            if (CategoryKey.IsEmpty)
                return null;

            return ("Year trends", Navigator.UrlTrends(CategoryKey));
        }
    }
}
