using Microsoft.AspNetCore.Authorization;
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
    [Route("/{Year:int}")]
    [Authorize]
    public class SummaryYear : Summary<YearModel>
    {
        [Parameter]
        public int? Year { get; set; }

        public SummaryYear() 
            => SubTitle = "Per-year summary of expenses in categories";

        protected override void ClearPreviousParameters() 
            => Year = null;

        protected override (YearModel, IReadOnlyCollection<YearModel>) CreateSelectedPeriodFromParameters()
        {
            Log.Debug($"CreateSelectedItemFromParameters(Year='{Year}')");

            YearModel period;
            if (Year != null)
                period = new YearModel(Year.Value);
            else
                period = DateTime.Now;

            return (period, new[] { period - 1, period - 2 });
        }

        protected override IQuery<List<YearModel>> CreatePeriodsQuery()
            => new ListYearWithOutcome();

        protected override IQuery<Price> CreateExpenseTotalQuery(YearModel item)
            => new GetTotalYearOutcome(item);

        protected override IQuery<List<CategoryWithAmountModel>> CreateCategoriesQuery(YearModel item)
            => new ListYearCategoryWithOutcome(item);

        protected override bool IsContained(DateTime changed)
            => Periods.Contains(changed);

        protected override string UrlSummary(YearModel item)
            => Navigator.UrlSummary(item);

        protected override void OpenOverview(YearModel item)
            => Navigator.OpenOverview(item);

        protected override void OpenOverview(YearModel item, IKey categorykey)
            => Navigator.OpenOverview(item, categorykey);
    }
}
