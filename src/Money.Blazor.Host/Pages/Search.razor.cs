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
    public partial class Search : OutcomeCard.IContext
    {
        public CurrencyFormatter CurrencyFormatter { get; private set; }

        [Inject]
        internal IQueryDispatcher Queries { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }

        protected LoadingContext Loading { get; } = new LoadingContext();
        protected SortDescriptor<OutcomeOverviewSortType> SortDescriptor { get; set; } = new SortDescriptor<OutcomeOverviewSortType>(OutcomeOverviewSortType.ByWhen, SortDirection.Descending);
        protected PagingContext PagingContext { get; set; }

        protected List<OutcomeOverviewModel> Models { get; set; }
        protected string Text { get; set; }

        protected async override Task OnInitializedAsync()
        {
            PagingContext = new PagingContext(LoadPageAsync, Loading);
            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();
        }

        protected Task OnSearchAsync()
        {
            Models = null;
            return PagingContext.LoadAsync(0);
        }

        protected async Task<PagingLoadStatus> LoadPageAsync()
        {
            if (!String.IsNullOrEmpty(Text))
            {
                var models = await Queries.QueryAsync(new SearchOutcomes(Text, SortDescriptor, PagingContext.CurrentPageIndex));
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

        protected async void OnSortChanged()
        {
            await PagingContext.LoadAsync(0);
            StateHasChanged();
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
