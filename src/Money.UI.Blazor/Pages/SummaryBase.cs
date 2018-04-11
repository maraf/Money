using Microsoft.AspNetCore.Blazor.Components;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
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
        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        public IQueryDispatcher Queries { get; set; }

        protected List<MonthModel> Months { get; private set; }
        protected MonthModel SelectedMonth { get; private set; }

        protected Price TotalAmout { get; private set; }
        protected List<CategoryWithAmountModel> Categories { get; private set; }

        protected override async Task OnInitAsync()
        {
            BindEvents();
            await LoadMonthsAsync();
        }

        private async void OnEvent()
        {
            if (SelectedMonth == null)
                await LoadMonthsAsync();
            else
                await LoadMonthSummaryAsync();

            StateHasChanged();
        }

        private async void OnMonthUpdatedEvent(MonthModel changed)
        {
            if (!Months.Contains(changed))
                await LoadMonthsAsync(changed);
            else if (SelectedMonth == changed)
                await LoadMonthSummaryAsync();

            StateHasChanged();
        }

        protected async Task LoadMonthsAsync(MonthModel selected = null)
        {
            Months = await Queries.QueryAsync(new ListMonthWithOutcome());

            if (selected != null)
                OnMonthSelected(selected);
            else if (SelectedMonth == null)
                OnMonthSelected(Months.FirstOrDefault());
        }

        protected async Task LoadMonthSummaryAsync()
        {
            if (SelectedMonth != null)
            {
                Categories = await Queries.QueryAsync(new ListMonthCategoryWithOutcome(SelectedMonth));
                TotalAmout = await Queries.QueryAsync(new GetTotalMonthOutcome(SelectedMonth));
            }
        }

        protected async void OnMonthSelected(MonthModel selectedMonth)
        {
            SelectedMonth = selectedMonth;
            await LoadMonthSummaryAsync();
            StateHasChanged();
        }

        protected decimal GetPercentualValue(CategoryWithAmountModel category)
        {
            decimal total = Categories.Sum(c => c.TotalAmount.Value);
            return 100 / total * category.TotalAmount.Value;
        }

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
            OnEvent();
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload)
        {
            OnEvent();
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload)
        {
            OnMonthUpdatedEvent(payload.When);
            return Task.CompletedTask;
        }

        #endregion
    }
}
