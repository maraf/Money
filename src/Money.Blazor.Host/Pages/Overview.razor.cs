using Microsoft.AspNetCore.Components;
using Money.Commands;
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
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class Overview<T> :
        System.IDisposable,
        IEventHandler<OutcomeCreated>,
        IEventHandler<OutcomeDeleted>,
        IEventHandler<OutcomeAmountChanged>,
        IEventHandler<OutcomeDescriptionChanged>,
        IEventHandler<OutcomeWhenChanged>,
        IEventHandler<PulledToRefresh>
    {
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

        protected string Title { get; set; }
        protected string SubTitle { get; set; }

        protected T SelectedPeriod { get; set; }
        protected IKey CategoryKey { get; set; }
        protected string CategoryName { get; set; }
        protected List<OutcomeOverviewModel> Items { get; set; }

        protected LoadingContext Loading { get; } = new LoadingContext();
        protected SortDescriptor<OutcomeOverviewSortType> SortDescriptor { get; set; } = new SortDescriptor<OutcomeOverviewSortType>(OutcomeOverviewSortType.ByWhen, SortDirection.Descending);
        protected PagingContext PagingContext { get; set; }

        protected override async Task OnInitializedAsync()
        {
            PagingContext = new PagingContext(LoadDataAsync, Loading);

            BindEvents();

            CategoryKey = CreateSelectedCategoryFromParameters();
            SelectedPeriod = CreateSelectedItemFromParameters();

            if (!CategoryKey.IsEmpty)
            {
                CategoryName = await Queries.QueryAsync(new GetCategoryName(CategoryKey));
                Title = $"{CategoryName} expenses in {SelectedPeriod}";
            }
            else
            {
                Title = $"Expenses in {SelectedPeriod}";
            }
            
            Reload();
        }

        protected virtual IKey CreateSelectedCategoryFromParameters()
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateSelectedCategoryFromParameters)}'.");

        protected virtual T CreateSelectedItemFromParameters()
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateSelectedItemFromParameters)}'.");

        protected virtual string ListIncomeUrl()
            => null;

        protected virtual string TrendsSelectedPeriodUrl()
            => null;

        protected virtual (string title, string url)? TrendsTitleUrl()
            => null;

        protected async void Reload()
        {
            await PagingContext.LoadAsync(0);
            StateHasChanged();
        }

        protected async Task<PagingLoadStatus> LoadDataAsync()
        {
            await Interop.ScrollToTopAsync();

            List<OutcomeOverviewModel> models = await Queries.QueryAsync(CreateItemsQuery(PagingContext.CurrentPageIndex));
            if (models.Count == 0)
                return PagingLoadStatus.EmptyPage;

            Items = models;
            return Items.Count == 10 ? PagingLoadStatus.HasNextPage : PagingLoadStatus.LastPage;
        }

        protected virtual IQuery<List<OutcomeOverviewModel>> CreateItemsQuery(int pageIndex)
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateItemsQuery)}'.");

        protected async void OnSortChanged()
        {
            await PagingContext.LoadAsync(0);
            StateHasChanged();
        }

        protected OutcomeOverviewModel FindModel(IEvent payload)
            => Items.FirstOrDefault(o => o.Key.Equals(payload.AggregateKey));

        public void Dispose()
            => UnBindEvents();

        #region Events

        private void BindEvents()
        {
            EventHandlers
                .Add<OutcomeCreated>(this)
                .Add<OutcomeDeleted>(this)
                .Add<OutcomeAmountChanged>(this)
                .Add<OutcomeDescriptionChanged>(this)
                .Add<OutcomeWhenChanged>(this)
                .Add<PulledToRefresh>(this);
        }

        private void UnBindEvents()
        {
            EventHandlers
                .Remove<OutcomeCreated>(this)
                .Remove<OutcomeDeleted>(this)
                .Remove<OutcomeAmountChanged>(this)
                .Remove<OutcomeDescriptionChanged>(this)
                .Remove<OutcomeWhenChanged>(this)
                .Remove<PulledToRefresh>(this);
        }

        private Task UpdateModel(IEvent payload, Action<OutcomeOverviewModel> handler)
        {
            OutcomeOverviewModel model = FindModel(payload);
            if (model != null)
            {
                handler(model);
                StateHasChanged();
            }

            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload)
        {
            if (IsContained(payload.When))
                Reload();

            return Task.CompletedTask;
        }

        protected virtual bool IsContained(DateTime when)
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(IsContained)}'.");

        Task IEventHandler<OutcomeDeleted>.HandleAsync(OutcomeDeleted payload)
        {
            Reload();
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload)
        {
            if (SortDescriptor.Type == OutcomeOverviewSortType.ByAmount)
                Reload();
            else
                UpdateModel(payload, model => model.Amount = payload.NewValue);

            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeDescriptionChanged>.HandleAsync(OutcomeDescriptionChanged payload)
        {
            if (SortDescriptor.Type == OutcomeOverviewSortType.ByDescription)
                Reload();
            else
                UpdateModel(payload, model => model.Description = payload.Description);

            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload)
        {
            if (SortDescriptor.Type != OutcomeOverviewSortType.ByWhen)
            {
                OutcomeOverviewModel model = FindModel(payload);
                if (model != null && model.When.Year == payload.When.Year && model.When.Month == payload.When.Month)
                {
                    model.When = payload.When;
                    StateHasChanged();
                    return Task.CompletedTask;
                }
            }

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
