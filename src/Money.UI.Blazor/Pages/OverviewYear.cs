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
    public class OverviewYear : Overview<YearModel>
    {
        [Parameter]
        public int Year { get; set; }

        [Parameter]
        public Guid? CategoryGuid { get; set; }

        public OverviewYear() 
            => SubTitle = "List of each single outcome in selected year";

        protected override YearModel CreateSelectedItemFromParameters()
            => new YearModel(Year);

        protected override IKey CreateSelectedCategoryFromParameters()
            => CategoryGuid != null ? GuidKey.Create(CategoryGuid.Value, KeyFactory.Empty(typeof(Category)).Type) : KeyFactory.Empty(typeof(Category));

        protected override IQuery<List<OutcomeOverviewModel>> CreateItemsQuery(int pageIndex)
            => new ListYearOutcomeFromCategory(CategoryKey, SelectedPeriod, SortDescriptor, pageIndex);

        protected override bool IsContained(DateTime when)
            => SelectedPeriod == when;
    }
}
