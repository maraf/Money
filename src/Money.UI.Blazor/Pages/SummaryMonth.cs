using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Neptuo.Logging;
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
    [Route("/")]
    [Route("/{Year}/{Month}")]
    public class SummaryMonth : Summary<MonthModel>
    {
        [Parameter]
        protected string Year { get; set; }

        [Parameter]
        protected string Month { get; set; }

        protected override void ClearPreviousParameters()
        {
            Year = null;
            Month = null;
        }

        protected override MonthModel CreateSelectedItemFromParameters()
        {
            Log.Debug($"CreateSelectedItemFromParameters(Year='{Year}', Month='{Month}')");

            if (!String.IsNullOrEmpty(Year) && !String.IsNullOrEmpty(Month))
                return new MonthModel(Int32.Parse(Year), Int32.Parse(Month));
            else
                return null;
        }

        protected override IQuery<List<MonthModel>> CreateItemsQuery()
            => new ListMonthWithOutcome();

        protected override IQuery<Price> CreateTotalQuery(MonthModel item)
            => new GetTotalMonthOutcome(item);

        protected override IQuery<List<CategoryWithAmountModel>> CreateCategoriesQuery(MonthModel item)
            => new ListMonthCategoryWithOutcome(item);

        protected override bool IsContained(DateTime changed)
            => Items.Contains(changed);

        protected override string UrlSummary(MonthModel item)
            => Navigator.UrlSummary(item);

        protected override void OpenOverview(MonthModel item)
            => Navigator.OpenOverview(item);

        protected override void OpenOverview(MonthModel item, IKey categorykey)
            => Navigator.OpenOverview(item, categorykey);
    }
}
