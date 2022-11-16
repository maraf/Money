using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class BalancesMonth
    {
        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected Navigator Navigator { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }

        [Parameter]
        public int Year { get; set; }

        protected CurrencyFormatter CurrencyFormatter { get; set; }
        protected YearModel SelectedPeriod { get; set; }
        protected List<MonthBalanceModel> Models { get; set; }
        protected decimal MaxAmount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            SelectedPeriod = new YearModel(Year);
            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();

            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            string defaultCurrency = await Queries.QueryAsync(new FindCurrencyDefault());
            var models = await Queries.QueryAsync(new ListMonthBalance(SelectedPeriod));

            MaxAmount = models.Max(m => Math.Max(m.IncomeSummary.Value, m.ExpenseSummary.Value));
            Models = new List<MonthBalanceModel>();
            for (int i = 0; i < 12; i++)
            {
                int month = i + 1;
                var model = models.FirstOrDefault(m => m.Month == month);
                if (model == null)
                    model = new MonthBalanceModel(Year, month, Price.Zero(defaultCurrency), Price.Zero(defaultCurrency));

                Models.Add(model);
            }
        }
    }
}
