using Microsoft.AspNetCore.Components;
using Neptuo;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class Paging
    {
        [Inject]
        internal ILog<Paging> Log { get; set; }

        [Parameter]
        public PagingContext Context { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Context == null)
                throw Ensure.Exception.Argument("Context", "Missing required parameter 'Context'.");
        }

        protected async Task OnPrevPageClickAsync()
        {
            await Context.PrevAsync();
            Log.Debug($"Data loaded (prev), hasNextPage='{Context.HasNextPage}', current index '{Context.CurrentPageIndex}'.");
            StateHasChanged();
        }

        protected async Task OnNextPageClickAsync()
        {
            await Context.NextAsync();
            Log.Debug($"Data loaded (next), hasNextPage='{Context.HasNextPage}', current index '{Context.CurrentPageIndex}'.");
            StateHasChanged();
        }
    }
}
