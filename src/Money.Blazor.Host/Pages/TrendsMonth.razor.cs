using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
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

        [Inject]
        protected Navigator Navigator { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }

        [Parameter]
        public int Year { get; set; }

        [Parameter]
        public Guid CategoryGuid { get; set; }

        protected CurrencyFormatter CurrencyFormatter { get; set; }
        protected YearModel SelectedPeriod { get; set; }
        protected IKey CategoryKey { get; set; }
        protected string CategoryName { get; set; }
        protected Color CategoryColor { get; set; }
        protected List<MonthWithAmountModel> Models { get; set; }
        protected decimal MaxAmount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            SelectedPeriod = new YearModel(Year);
            CategoryKey = GuidKey.Create(CategoryGuid, KeyFactory.Empty(typeof(Category)).Type);
            CategoryName = await Queries.QueryAsync(new GetCategoryName(CategoryKey));
            CategoryColor = await Queries.QueryAsync(new GetCategoryColor(CategoryKey));
            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();

            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            string defaultCurrency = await Queries.QueryAsync(new FindCurrencyDefault());
            var models = await Queries.QueryAsync(new ListMonthOutcomesForCategory(CategoryKey, SelectedPeriod));

            MaxAmount = 0;
            Models = new List<MonthWithAmountModel>();
            for (int i = 0; i < 12; i++)
            {
                int month = i + 1;
                var model = models.FirstOrDefault(m => m.Month == month);
                if (model == null)
                    model = new MonthWithAmountModel(Year, month, Price.Zero(defaultCurrency));

                if (model.TotalAmount.Value > MaxAmount)
                    MaxAmount = model.TotalAmount.Value;

                Models.Add(model);
            }
        }
    }
}
