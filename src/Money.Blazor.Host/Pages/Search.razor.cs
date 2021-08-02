using Microsoft.AspNetCore.Components;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Services;
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
    public partial class Search : System.IDisposable,
        IEventHandler<OutcomeCreated>,
        IEventHandler<OutcomeDeleted>,
        IEventHandler<OutcomeAmountChanged>,
        IEventHandler<OutcomeDescriptionChanged>,
        IEventHandler<OutcomeWhenChanged>,
        IEventHandler<PulledToRefresh>
    {
        public static readonly SortDescriptor<OutcomeOverviewSortType> DefaultSort = new SortDescriptor<OutcomeOverviewSortType>(OutcomeOverviewSortType.ByWhen, SortDirection.Descending);

        public CurrencyFormatter CurrencyFormatter { get; private set; }

        [Inject]
        protected Navigator Navigator { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }

        [Parameter]
        public string Query { get; set; }

        protected LoadingContext Loading { get; } = new LoadingContext();
        protected SortDescriptor<OutcomeOverviewSortType> Sort { get; set; }
        protected PagingContext PagingContext { get; set; }

        protected List<OutcomeOverviewModel> Models { get; set; }
        protected string FormText { get; set; }
        protected SortDescriptor<OutcomeOverviewSortType> FormSort { get; set; }

        protected async override Task OnInitializedAsync()
        {
            FormSort = Sort = DefaultSort;
            PagingContext = new PagingContext(LoadPageAsync, Loading);
            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();
            Navigator.LocationChanged += OnLocationChanged;
            BindEvents();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await OnSearchAsync();
        }

        public void Dispose()
        {
            Navigator.LocationChanged -= OnLocationChanged;
            UnBindEvents();
        }

        private async void OnLocationChanged(string url)
        {
            await OnSearchAsync();
            StateHasChanged();
        }

        protected Task OnSearchAsync()
        {
            string lastQuery = Query;
            var lastSort = Sort;
            FormText = Query = Navigator.FindQueryParameter("q");

            if (SortDescriptor.TryParseFromUrl<OutcomeOverviewSortType>(Navigator.FindQueryParameter("sort"), out var descriptor))
                FormSort = Sort = descriptor;
            else
                FormSort = Sort = DefaultSort;

            Console.WriteLine($"Sort: last '{lastSort.Type}+{lastSort.Direction}', current '{Sort.Type}+{Sort.Direction}'.");

            if (lastQuery == Query && lastSort.Equals(Sort))
                return Task.CompletedTask;

            return PagingContext.LoadAsync(0);
        }

        protected async Task<PagingLoadStatus> LoadPageAsync()
        {
            if (!String.IsNullOrEmpty(FormText))
            {
                var models = await Queries.QueryAsync(new SearchOutcomes(FormText, Sort, PagingContext.CurrentPageIndex));
                if (models.Count == 0)
                    return PagingLoadStatus.EmptyPage;

                Models = models;
                return Models.Count == 10 ? PagingLoadStatus.HasNextPage : PagingLoadStatus.LastPage;
            }
            else
            {
                Models.Clear();
                return PagingLoadStatus.LastPage;
            }
        }

        protected OutcomeOverviewModel FindModel(IEvent payload)
            => Models.FirstOrDefault(o => o.Key.Equals(payload.AggregateKey));

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
            _ = LoadPageAsync();
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeDeleted>.HandleAsync(OutcomeDeleted payload)
        {
            _ = LoadPageAsync();
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload)
        {
            if (Sort.Type == OutcomeOverviewSortType.ByAmount)
                _ = LoadPageAsync();
            else
                UpdateModel(payload, model => model.Amount = payload.NewValue);

            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeDescriptionChanged>.HandleAsync(OutcomeDescriptionChanged payload)
        {
            _ = LoadPageAsync();
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload)
        {
            if (Sort.Type == OutcomeOverviewSortType.ByWhen)
                _ = LoadPageAsync();
            else
                UpdateModel(payload, model => model.When = payload.When);

            return Task.CompletedTask;
        }

        async Task IEventHandler<PulledToRefresh>.HandleAsync(PulledToRefresh payload)
        {
            payload.IsHandled = true;
            await LoadPageAsync();
            StateHasChanged();
        }

        #endregion
    }
}
