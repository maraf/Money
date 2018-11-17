using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using Money.Models.Loading;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public class LoadingBase : BlazorComponent
    {
        [Inject]
        internal ILog<LoadingBase> Log { get; set; }

        [Parameter]
        protected LoadingContext Context { get; set; }

        [Parameter]
        protected bool IsOverlay { get; set; }

        protected bool IsLoading { get; set; }

        [Parameter]
        protected RenderFragment ChildContent { get; set; }

        public override void SetParameters(ParameterCollection parameters)
        {
            if (Context != null)
                Context.LoadingChanged -= OnLoadingChanged;

            base.SetParameters(parameters);

            if (Context != null)
            {
                Context.LoadingChanged += OnLoadingChanged;
                OnLoadingChanged(Context);
            }
        }

        protected void OnLoadingChanged(LoadingContext context)
        {
            Log.Debug($"Loading changed to '{context.IsLoading}'.");
            IsLoading = Context.IsLoading;
            StateHasChanged();
        }
    }
}
