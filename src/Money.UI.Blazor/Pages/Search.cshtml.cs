using Microsoft.AspNetCore.Blazor.Components;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public class SearchBase : BlazorComponent
    {
        private CurrencyFormatter formatter;

        [Inject]
        internal IQueryDispatcher Queries { get; set; }

        protected LoadingContext Loading { get; } = new LoadingContext();
        protected List<OutcomeSearchModel> Models { get; set; }
        protected List<CategoryModel> Categories { get; set; }
        protected string Text { get; set; }

        protected async override Task OnInitAsync()
        {
            Categories = await Queries.QueryAsync(new ListAllCategory());
            formatter = new CurrencyFormatter(await Queries.QueryAsync(new ListAllCurrency()));
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

        protected string FormatPrice(Price price)
            => formatter.Format(price);
    }
}
