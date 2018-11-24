using Microsoft.AspNetCore.Blazor.Components;
using Money.Models;
using Money.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public class OutcomeCardBase : BlazorComponent
    {
        public interface IContext
        {
            CurrencyFormatter CurrencyFormatter { get; }

            void EditAmount(OutcomeOverviewModel model);
            void EditDescription(OutcomeOverviewModel model);
            void EditWhen(OutcomeOverviewModel model);
            void Delete(OutcomeOverviewModel model);
        }

        [Parameter]
        [CascadingParameter]
        protected IContext Context { get; set; }

        [Parameter]
        protected OutcomeOverviewModel Model { get; set; }

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
