using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.Services;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public class SummaryBase : BlazorComponent,
        IDisposable,
        IEventHandler<OutcomeCreated>,
        IEventHandler<OutcomeDeleted>,
        IEventHandler<OutcomeAmountChanged>,
        IEventHandler<OutcomeWhenChanged>
    {
        private CurrencyFormatter formatter;

        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        public IQueryDispatcher Queries { get; set; }

        [Inject]
        internal ILog<SummaryBase> Log { get; set; }

        [Inject]
        internal Navigator Navigator { get; set; }

        [Parameter]
        protected string Year { get; set; }

        [Parameter]
        protected string Month { get; set; }

        protected List<MonthModel> Months { get; private set; }
        protected MonthModel SelectedMonth { get; private set; }

        protected Price TotalAmout { get; private set; }
        protected List<CategoryWithAmountModel> Categories { get; private set; }

        protected LoadingContext Loading { get; } = new LoadingContext();

        protected bool IsCreateVisible { get; set; }

        protected override Task OnInitAsync()
        {
            Log.Debug("Summary.OnInitAsync");
            BindEvents();

            return base.OnInitAsync();
        }

        public override void SetParameters(ParameterCollection parameters)
        {
            // Clear previous parameter values.
            Year = null;
            Month = null;

            base.SetParameters(parameters);
        }

        protected async override Task OnParametersSetAsync()
        {
            Log.Debug($"Summary.OnParametersSetAsync(Year='{Year}', Month='{Month}')");

            if (!String.IsNullOrEmpty(Year) && !String.IsNullOrEmpty(Month))
                SelectedMonth = new MonthModel(Int32.Parse(Year), Int32.Parse(Month));
            else
                SelectedMonth = null;

            await LoadMonthsAsync(isReload: false);
        }

        protected async Task LoadMonthsAsync(bool isReload = true)
        {
            if (isReload || Months == null)
            {
                using (Loading.Start())
                    Months = await Queries.QueryAsync(new ListMonthWithOutcome());
            }

            if (SelectedMonth != null && !Months.Contains(SelectedMonth))
            {
                Navigator.OpenSummary();
                return;
            }

            if (SelectedMonth == null)
                SelectedMonth = Months.FirstOrDefault();

            await LoadMonthSummaryAsync();
        }

        protected async Task LoadMonthSummaryAsync()
        {
            if (SelectedMonth != null)
            {
                Categories = await Queries.QueryAsync(new ListMonthCategoryWithOutcome(SelectedMonth));
                TotalAmout = await Queries.QueryAsync(new GetTotalMonthOutcome(SelectedMonth));
                formatter = new CurrencyFormatter(await Queries.QueryAsync(new ListAllCurrency()));
            }
        }

        protected decimal GetPercentualValue(CategoryWithAmountModel category)
        {
            decimal total = Categories.Sum(c => c.TotalAmount.Value);
            return 100 / total * category.TotalAmount.Value;
        }

        protected string FormatPrice(Price price)
            => formatter.Format(price);

        public void Dispose()
            => UnBindEvents();

        #region Events

        private void BindEvents()
        {
            EventHandlers
                .Add<OutcomeCreated>(this)
                .Add<OutcomeDeleted>(this)
                .Add<OutcomeAmountChanged>(this)
                .Add<OutcomeWhenChanged>(this);
        }

        private void UnBindEvents()
        {
            EventHandlers
                .Remove<OutcomeCreated>(this)
                .Remove<OutcomeDeleted>(this)
                .Remove<OutcomeAmountChanged>(this)
                .Remove<OutcomeWhenChanged>(this);
        }

        Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload)
        {
            OnMonthUpdatedEvent(payload.When);
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeDeleted>.HandleAsync(OutcomeDeleted payload)
        {
            OnOutcomeDeletedEvent();
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload)
        {
            OnOutcomeAmountChangedEvent();
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload)
        {
            OnMonthUpdatedEvent(payload.When);
            return Task.CompletedTask;
        }

        private async void OnMonthUpdatedEvent(MonthModel changed)
        {
            if (!Months.Contains(changed))
                await LoadMonthsAsync();
            else
                await LoadMonthSummaryAsync();

            StateHasChanged();
        }

        private async void OnOutcomeAmountChangedEvent()
        {
            await LoadMonthSummaryAsync();
            StateHasChanged();
        }

        private async void OnOutcomeDeletedEvent()
        {
            await LoadMonthsAsync();
            StateHasChanged();
        }

        #endregion
    }
}
