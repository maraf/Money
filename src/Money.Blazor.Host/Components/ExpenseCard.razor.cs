using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class ExpenseCard
    {
        public interface IContext
        {
            bool HasEdit { get; }

            CurrencyFormatter CurrencyFormatter { get; }

            void EditAmount(OutcomeOverviewModel model);
            void EditDescription(OutcomeOverviewModel model);
            void EditWhen(OutcomeOverviewModel model);
            void Delete(OutcomeOverviewModel model);
        }

        [Inject]
        internal IQueryDispatcher Queries { get; set; }

        [Parameter]
        [CascadingParameter]
        public IContext Context { get; set; }

        [Parameter]
        public OutcomeOverviewModel Model { get; set; }

        protected string CategoryName { get; private set; }
        protected Color CategoryColor { get; private set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            CategoryName = await Queries.QueryAsync(new GetCategoryName(Model.CategoryKey));
            CategoryColor = await Queries.QueryAsync(new GetCategoryColor(Model.CategoryKey));
        }

        protected void OnEditAmount() 
            => Context.EditAmount(Model);

        protected void OnEditDescription() 
            => Context.EditDescription(Model);

        protected void OnEditWhen() 
            => Context.EditWhen(Model);

        protected void OnDelete() 
            => Context.Delete(Model);
    }
}
