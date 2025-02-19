using Microsoft.AspNetCore.Components;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Queries;
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
    public partial class BalancesMonth : IDisposable,
        IEventHandler<SwipedLeft>,
        IEventHandler<SwipedRight>
    {
        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected Navigator Navigator { get; set; }

        [Inject]
        protected IEventHandlerCollection EventHandlers { get; set; }

        [Parameter]
        public int Year { get; set; }

        protected YearModel SelectedPeriod { get; set; }
        protected BalanceDisplayType SelectedDisplayType { get; set; }
        protected bool IncludeExpectedExpenses { get; set; } = true;
        protected List<YearModel> PeriodGuesses { get; set; }
        protected List<MonthBalanceModel> Models { get; set; }
        protected decimal MaxAmount { get; set; }
        protected Price TotalExpenses { get; set; }
        protected Price TotalExpectedExpenses { get; set; }
        protected Price TotalIncomes { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            SelectedDisplayType = await Queries.QueryAsync(new GetBalanceDisplayProperty());

            EventHandlers
                .Add<SwipedLeft>(this)
                .Add<SwipedRight>(this);
        }

        public void Dispose() 
        {
            EventHandlers
                .Remove<SwipedLeft>(this)
                .Remove<SwipedRight>(this);
        }

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            SelectedPeriod = new YearModel(Year);
            PeriodGuesses = new List<YearModel>(2)
            {
                SelectedPeriod - 1,
                SelectedPeriod - 2,
            };

            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            string defaultCurrency = await Queries.QueryAsync(new FindCurrencyDefault());
            var models = await Queries.QueryAsync(new ListMonthBalance(SelectedPeriod, includeExpectedExpenses: IncludeExpectedExpenses));

            MaxAmount = models.Count > 0 ? models.Max(m => Math.Max(m.IncomeSummary.Value, m.ExpenseSummary.Value + m.ExpectedExpenseSummary?.Value ?? 0)) : 0;
            Models = [];
            TotalExpenses = Price.Zero(defaultCurrency);
            TotalIncomes = Price.Zero(defaultCurrency);
            TotalExpectedExpenses = Price.Zero(defaultCurrency);
            for (int i = 0; i < 12; i++)
            {
                int month = i + 1;
                var model = models.FirstOrDefault(m => m.Month == month);
                if (model == null)
                    model = new MonthBalanceModel(Year, month, Price.Zero(defaultCurrency), Price.Zero(defaultCurrency));

                TotalExpenses += model.ExpenseSummary;
                if (model.ExpectedExpenseSummary != null)
                    TotalExpectedExpenses += model.ExpectedExpenseSummary;
                
                TotalIncomes += model.IncomeSummary;
                Models.Add(model);
            }
        }

        private async void ReloadAsync()
        {
            await LoadAsync();
            StateHasChanged();
        }

        protected async Task<IReadOnlyCollection<YearModel>> GetYearsAsync() 
            => await Queries.QueryAsync(new ListYearWithOutcome());

        Task IEventHandler<SwipedLeft>.HandleAsync(SwipedLeft payload)
        {
            Navigator.OpenBalances(SelectedPeriod - 1);
            return Task.CompletedTask;
        }

        Task IEventHandler<SwipedRight>.HandleAsync(SwipedRight payload)
        {
            Navigator.OpenBalances(SelectedPeriod + 1);
            return Task.CompletedTask;
        }
    }
}
