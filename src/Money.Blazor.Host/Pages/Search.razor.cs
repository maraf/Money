using Microsoft.AspNetCore.Components;
using Money.Components;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Services;
using Neptuo;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class Search : OutcomeCard.IContext, System.IDisposable
    {
        public static readonly SortDescriptor<OutcomeOverviewSortType> DefaultSort = new SortDescriptor<OutcomeOverviewSortType>(OutcomeOverviewSortType.ByWhen, SortDirection.Descending);

        public CurrencyFormatter CurrencyFormatter { get; private set; }

        [Inject]
        protected Navigator Navigator { get; set; }

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
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await OnSearchAsync();
        }

        public void Dispose()
        {
            Navigator.LocationChanged -= OnLocationChanged;
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

        #region OutcomeCard.IContext

        bool OutcomeCard.IContext.HasEdit => false;

        void OutcomeCard.IContext.EditAmount(OutcomeOverviewModel model)
            => throw Ensure.Exception.NotSupported();

        void OutcomeCard.IContext.EditDescription(OutcomeOverviewModel model)
            => throw Ensure.Exception.NotSupported();

        void OutcomeCard.IContext.EditWhen(OutcomeOverviewModel model)
            => throw Ensure.Exception.NotSupported();

        void OutcomeCard.IContext.Delete(OutcomeOverviewModel model)
            => throw Ensure.Exception.NotSupported();

        #endregion
    }
}
