using Microsoft.AspNetCore.Blazor.Components;
using Money.Components;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public class SearchBase : BlazorComponent,
        OutcomeCardBase.IContext
    {
        public CurrencyFormatter CurrencyFormatter { get; private set; }

        [Inject]
        internal IQueryDispatcher Queries { get; set; }

        protected LoadingContext Loading { get; } = new LoadingContext();
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
                using (Loading.Start())
                {
                    var models = await Queries.QueryAsync(new SearchOutcomes(Text, pageIndex));
                    if (models.Count == 0 && pageIndex > 0)
                        return false;

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

        #region OutcomeCardBase.IContext

        void OutcomeCardBase.IContext.EditAmount(OutcomeOverviewModel model)
        {
            throw new NotImplementedException();
        }

        void OutcomeCardBase.IContext.EditDescription(OutcomeOverviewModel model)
        {
            throw new NotImplementedException();
        }

        void OutcomeCardBase.IContext.EditWhen(OutcomeOverviewModel model)
        {
            throw new NotImplementedException();
        }

        void OutcomeCardBase.IContext.Delete(OutcomeOverviewModel model)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
