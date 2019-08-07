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
    [Route("/outcomes/{Year}/{Month}")]
    [Route("/overview/{Year}/{Month}")]
    [Route("/overview/{Year}/{Month}/{CategoryGuid}")]
    [Route("/{Year}/{Month}/overview")]
    [Route("/{Year}/{Month}/overview/{CategoryGuid}")]
    public class OverviewMonth : Overview<MonthModel>
    {
        [Parameter]
        protected string Year { get; set; }

        [Parameter]
        protected string Month { get; set; }

        [Parameter]
        protected string CategoryGuid { get; set; }

        public OverviewMonth() 
            => SubTitle = "List of each single outcome in selected month";

        protected override MonthModel CreateSelectedItemFromParameters() 
            => new MonthModel(Int32.Parse(Year), Int32.Parse(Month));

        protected override IKey CreateSelectedCategoryFromParameters()
            => Guid.TryParse(CategoryGuid, out var categoryGuid) ? GuidKey.Create(categoryGuid, KeyFactory.Empty(typeof(Category)).Type) : KeyFactory.Empty(typeof(Category));

        protected override IQuery<List<OutcomeOverviewModel>> CreateItemsQuery(int pageIndex) 
            => new ListMonthOutcomeFromCategory(CategoryKey, SelectedPeriod, SortDescriptor, pageIndex);

        protected override bool IsContained(DateTime when) 
            => SelectedPeriod == when;
    }
}
