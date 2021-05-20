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
    public partial class TrendsYear
    {
        public const int ModelsCount = 10;

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected Navigator Navigator { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }

        [Parameter]
        public Guid CategoryGuid { get; set; }

        protected YearModel StartYear { get; set; }
        protected CurrencyFormatter CurrencyFormatter { get; set; }
        protected IKey CategoryKey { get; set; }
        protected string CategoryName { get; set; }
        protected Color CategoryColor { get; set; }
        protected List<YearWithAmountModel> Models { get; set; }
        protected decimal MaxAmount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            StartYear = new YearModel(DateTime.Today.Year - 8);
            CategoryKey = GuidKey.Create(CategoryGuid, KeyFactory.Empty(typeof(Category)).Type);
            CategoryName = await Queries.QueryAsync(new GetCategoryName(CategoryKey));
            CategoryColor = await Queries.QueryAsync(new GetCategoryColor(CategoryKey));
            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();

            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            string defaultCurrency = await Queries.QueryAsync(new FindCurrencyDefault());
            var models = await Queries.QueryAsync(new ListYearOutcomesForCategory(CategoryKey, StartYear));

            MaxAmount = 0;
            Models = new List<YearWithAmountModel>();
            for (int i = 0; i < ModelsCount; i++)
            {
                var year = StartYear.Year + i;
                var model = models.FirstOrDefault(m => m.Year == year);
                if (model == null)
                    model = new YearWithAmountModel(year, Price.Zero(defaultCurrency));

                if (model.TotalAmount.Value > MaxAmount)
                    MaxAmount = model.TotalAmount.Value;

                Models.Add(model);
            }
        }
    }
}
