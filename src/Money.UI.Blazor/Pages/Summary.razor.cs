using Microsoft.AspNetCore.Components;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
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
    public partial class Summary<T> : 
        System.IDisposable,
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
        internal ILog<Summary<T>> Log { get; set; }

        [Inject]
        internal Navigator Navigator { get; set; }

        protected string SubTitle { get; set; }

        protected List<T> Items { get; private set; }
        protected T SelectedItem { get; set; }

        protected Price TotalAmout { get; private set; }
        protected List<CategoryWithAmountModel> Categories { get; private set; }

        protected LoadingContext Loading { get; } = new LoadingContext();
        protected SortDescriptor<SummarySortType> SortDescriptor { get; set; }

        protected ModalDialog CreateModal { get; set; }

        protected override Task OnInitializedAsync()
        {
            Log.Debug("Summary.OnInitializedAsync");
            BindEvents();
            SortDescriptor = new SortDescriptor<SummarySortType>(SummarySortType.ByCategory, SortDirection.Ascending);

            return base.OnInitializedAsync();
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            ClearPreviousParameters();
            return base.SetParametersAsync(parameters);
        }

        protected virtual void ClearPreviousParameters() 
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(ClearPreviousParameters)}'.");

        protected virtual T CreateSelectedItemFromParameters()
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateSelectedItemFromParameters)}'.");

        protected async override Task OnParametersSetAsync()
        {
            SelectedItem = CreateSelectedItemFromParameters();
            await LoadItemsAsync(isReload: false);
        }

        protected virtual IQuery<List<T>> CreateItemsQuery()
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateItemsQuery)}'.");

        protected async Task LoadItemsAsync(bool isReload = true)
        {
            if (isReload || Items == null)
            {
                using (Loading.Start())
                    Items = await Queries.QueryAsync(CreateItemsQuery());
            }

            if (SelectedItem != null && !Items.Contains(SelectedItem))
            {
                Navigator.OpenSummary();
                return;
            }

            if (SelectedItem == null)
                SelectedItem = Items.FirstOrDefault();

            await LoadItemSummaryAsync();
        }

        protected async Task LoadItemSummaryAsync()
        {
            if (SelectedItem != null)
            {
                Categories = await Queries.QueryAsync(CreateCategoriesQuery(SelectedItem));
                TotalAmout = await Queries.QueryAsync(CreateTotalQuery(SelectedItem));
                formatter = new CurrencyFormatter(await Queries.QueryAsync(new ListAllCurrency()));
                Sort();
            }
        }

        protected virtual IQuery<Price> CreateTotalQuery(T item)
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateTotalQuery)}'.");

        protected virtual IQuery<List<CategoryWithAmountModel>> CreateCategoriesQuery(T item)
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateCategoriesQuery)}'.");

        protected void OnSorted()
        {
            Sort();
            StateHasChanged();
        }

        private void Sort()
        {
            Log.Debug($"Sorting: Type='{SortDescriptor.Type}', Direction='{SortDescriptor.Direction}'.");

            switch (SortDescriptor.Type)
            {
                case SummarySortType.ByAmount:
                    Categories.Sort(SortDescriptor.Direction, x => x.TotalAmount.Value);
                    break;
                case SummarySortType.ByCategory:
                    Categories.Sort(SortDescriptor.Direction, x => x.Name);
                    break;
                default:
                    throw Ensure.Exception.NotSupported(SortDescriptor.Type.ToString());
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

        #region Navigations

        protected virtual string UrlSummary(T item) => null;

        protected virtual void OpenOverview(T item)
        { }

        protected virtual void OpenOverview(T item, IKey categorykey)
        { }

        #endregion

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

        private async void OnMonthUpdatedEvent(DateTime changed)
        {
            if (!IsContained(changed))
                await LoadItemsAsync();
            else
                await LoadItemSummaryAsync();

            StateHasChanged();
        }

        protected virtual bool IsContained(DateTime changed)
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(IsContained)}'.");

        private async void OnOutcomeAmountChangedEvent()
        {
            await LoadItemSummaryAsync();
            StateHasChanged();
        }

        private async void OnOutcomeDeletedEvent()
        {
            await LoadItemsAsync();
            StateHasChanged();
        }

        #endregion
    }
}
