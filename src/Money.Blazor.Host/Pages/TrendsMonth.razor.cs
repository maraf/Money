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
    public partial class TrendsMonth
    {
        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Parameter]
        public int Year { get; set; }

        [Parameter]
        public Guid CategoryGuid { get; set; }

        protected YearModel SelectedPeriod { get; set; }
        protected IKey CategoryKey { get; set; }
        protected List<MonthWithAmountModel> Models { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            SelectedPeriod = new YearModel(Year);
            CategoryKey = GuidKey.Create(CategoryGuid, KeyFactory.Empty(typeof(Category)).Type);

            Models = await Queries.QueryAsync(new ListMonthOutcomesForCategory(CategoryKey, SelectedPeriod));
        }
    }
}
