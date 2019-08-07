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
    [Route("/{Year}/overview")]
    [Route("/{Year}/overview/{CategoryGuid}")]
    public class OverviewYear : Overview<YearModel>
    {
        [Parameter]
        protected string Year { get; set; }

        [Parameter]
        protected string CategoryGuid { get; set; }

        public OverviewYear() 
            => SubTitle = "List of each single outcome in selected year";

        protected override YearModel CreateSelectedItemFromParameters()
            => new YearModel(Int32.Parse(Year));

        protected override IKey CreateSelectedCategoryFromParameters()
            => Guid.TryParse(CategoryGuid, out var categoryGuid) ? GuidKey.Create(categoryGuid, KeyFactory.Empty(typeof(Category)).Type) : KeyFactory.Empty(typeof(Category));

        protected override IQuery<List<OutcomeOverviewModel>> CreateItemsQuery(int pageIndex)
            => new ListYearOutcomeFromCategory(CategoryKey, SelectedPeriod, SortDescriptor, pageIndex);

        protected override bool IsContained(DateTime when)
            => SelectedPeriod == when;
    }
}
