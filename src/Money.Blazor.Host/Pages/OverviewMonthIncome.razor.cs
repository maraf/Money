using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
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
    public partial class OverviewMonthIncome : IDisposable,
        IEventHandler<IncomeCreated>,
        IEventHandler<IncomeDeleted>,
        IEventHandler<PulledToRefresh>
    {
        public CurrencyFormatter CurrencyFormatter { get; private set; }

        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        public IQueryDispatcher Queries { get; set; }

        [Inject]
        public Interop Interop { get; set; }

        [Inject]
        public Navigator Navigator { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }

        [Parameter]
        public int Year { get; set; }

        [Parameter]
        public int Month { get; set; }

        protected MonthModel MonthModel { get; set; }

        protected List<IncomeOverviewModel> Items { get; set; }
        protected IncomeOverviewModel SelectedItem { get; set; }

        protected IncomeCreate CreateModal { get; set; }

        protected LoadingContext Loading { get; } = new LoadingContext();
        protected SortDescriptor<IncomeOverviewSortType> SortDescriptor { get; set; } = new SortDescriptor<IncomeOverviewSortType>(IncomeOverviewSortType.ByWhen, SortDirection.Descending);
        protected PagingContext PagingContext { get; set; }

        protected IncomeOverviewModel Selected { get; set; }
        protected string DeleteMessage { get; set; }
        protected Confirm DeleteConfirm { get; set; }

        protected override async Task OnInitializedAsync()
        {
            PagingContext = new PagingContext(LoadDataAsync, Loading);

            BindEvents();

            MonthModel = new MonthModel(Year, Month);

            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();
            Reload();
        }

        protected async void Reload()
        {
            await PagingContext.LoadAsync(0);
            StateHasChanged();
        }

        protected async Task<PagingLoadStatus> LoadDataAsync()
        {
            await Interop.ScrollToTopAsync();

            List<IncomeOverviewModel> models = await Queries.QueryAsync(new ListMonthIncome(MonthModel, SortDescriptor, PagingContext.CurrentPageIndex));
            if (models.Count == 0)
                return PagingLoadStatus.EmptyPage;

            Items = models;
            return Items.Count == 10 ? PagingLoadStatus.HasNextPage : PagingLoadStatus.LastPage;
        }

        protected async void OnSortChanged()
        {
            await PagingContext.LoadAsync(0);
            StateHasChanged();
        }

        protected void OnActionClick(IncomeOverviewModel model, ModalDialog modal)
        {
            SelectedItem = model;
            modal.Show();
            StateHasChanged();
        }

        protected void OnDeleteClick(IncomeOverviewModel model)
        {
            Selected = model;
            DeleteMessage = $"Do you really want to delete income '{model.Description}'?";
            DeleteConfirm.Show();
            StateHasChanged();
        }

        protected async void OnDeleteConfirmed()
        {
            await Commands.HandleAsync(new DeleteIncome(Selected.Key));
            StateHasChanged();
        }

        protected IncomeOverviewModel FindModel(IEvent payload)
            => Items.FirstOrDefault(o => o.Key.Equals(payload.AggregateKey));

        public void Dispose()
            => UnBindEvents();

        #region Events

        private void BindEvents()
        {
            EventHandlers
                .Add<IncomeCreated>(this)
                .Add<IncomeDeleted>(this)
                .Add<PulledToRefresh>(this);
        }

        private void UnBindEvents()
        {
            EventHandlers
                .Remove<IncomeCreated>(this)
                .Remove<IncomeDeleted>(this)
                .Remove<PulledToRefresh>(this);
        }

        Task IEventHandler<IncomeCreated>.HandleAsync(IncomeCreated payload)
        {
            if (MonthModel == payload.When)
                Reload();

            return Task.CompletedTask;
        }

        Task IEventHandler<IncomeDeleted>.HandleAsync(IncomeDeleted payload)
        {
            Reload();
            return Task.CompletedTask;
        }

        async Task IEventHandler<PulledToRefresh>.HandleAsync(PulledToRefresh payload)
        {
            payload.IsHandled = true;
            await LoadDataAsync();
            StateHasChanged();
        }

        #endregion
    }
}
