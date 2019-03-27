using Microsoft.AspNetCore.Components;
using Money.Components;
using Money.Models;
using Money.Models.Confirmation;
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
    public class SearchBase : ComponentBase,
        OutcomeCardBase.IContext
    {
        public CurrencyFormatter CurrencyFormatter { get; private set; }

        [Inject]
        internal IQueryDispatcher Queries { get; set; }

        protected LoadingContext Loading { get; } = new LoadingContext();
        protected SortDescriptor<OutcomeOverviewSortType> SortDescriptor { get; set; } = new SortDescriptor<OutcomeOverviewSortType>(OutcomeOverviewSortType.ByWhen, SortDirection.Descending);
        protected int CurrentPageIndex { get; set; }

        protected List<OutcomeOverviewModel> Models { get; set; }
        protected string Text { get; set; }

        protected async override Task OnInitAsync()
        {
            CurrencyFormatter = new CurrencyFormatter(await Queries.QueryAsync(new ListAllCurrency()));
        }

        protected async Task<bool> OnSearchAsync(int pageIndex = 0)
        {
            if (!String.IsNullOrEmpty(Text))
            {
                int prevPageIndex = CurrentPageIndex;
                CurrentPageIndex = pageIndex;
                using (Loading.Start())
                {
                    var models = await Queries.QueryAsync(new SearchOutcomes(Text, SortDescriptor, pageIndex));
                    if (models.Count == 0 && pageIndex > 0)
                    {
                        CurrentPageIndex = prevPageIndex;
                        return false;
                    }

                    Models = models;
                }

                return Models.Count > 0;
            }
            else
            {
                Models.Clear();
                return false;
            }
        }

        protected async void OnSortChanged()
        {
            await OnSearchAsync(CurrentPageIndex);
            StateHasChanged();
        }
        
        #region OutcomeCardBase.IContext

        void OutcomeCardBase.IContext.EditAmount(OutcomeOverviewModel model)
            => throw Ensure.Exception.NotSupported();

        void OutcomeCardBase.IContext.EditDescription(OutcomeOverviewModel model)
            => throw Ensure.Exception.NotSupported();

        void OutcomeCardBase.IContext.EditWhen(OutcomeOverviewModel model)
            => throw Ensure.Exception.NotSupported();

        void OutcomeCardBase.IContext.Delete(OutcomeOverviewModel model)
            => throw Ensure.Exception.NotSupported();

        #endregion
    }
}
