using Microsoft.AspNetCore.Blazor.Components;
using Neptuo;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public class PagingBase : BlazorComponent, System.IDisposable
    {
        [Inject]
        internal ILog<PagingBase> Log { get; set; }

        protected int CurrentIndex { get; set; }
        protected bool HasNextPage { get; set; } = true;

        [Parameter]
        protected Func<int, Task<bool>> LoadPageAsync { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (LoadPageAsync == null)
                throw Ensure.Exception.Argument("LoadPageAsync", "Missing required parameter 'LoadPageAsync'.");
        }

        protected Task OnPrevPageClickAsync()
        {
            if (CurrentIndex == 0)
                return Task.CompletedTask;

            CurrentIndex--;
            HasNextPage = true;
            return LoadPageAsync?.Invoke(CurrentIndex) ?? Task.CompletedTask;
        }

        protected async Task OnNextPageClickAsync()
        {
            if (!HasNextPage)
                return;

            bool hasNextPage = await LoadPageAsync(CurrentIndex + 1);
            Log.Debug($"Data loaded, hasNextPage='{hasNextPage}'.");
            if (hasNextPage)
                CurrentIndex++;
            else
                HasNextPage = false;
        }

        public void Dispose()
        {
            Log.Debug("Disposing.");
        }
    }
}
