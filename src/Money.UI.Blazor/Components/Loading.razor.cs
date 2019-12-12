using Microsoft.AspNetCore.Components;
using Money.Models.Loading;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class Loading
    {
        [Inject]
        internal ILog<Loading> Log { get; set; }

        [Parameter]
        public LoadingContext Context { get; set; }

        [Parameter]
        public bool IsOverlay { get; set; }

        protected bool IsLoading { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment LoadingContent { get; set; } = r => r.AddMarkupContent(1, "<i>Loading...</i>");

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (Context != null)
                Context.LoadingChanged -= OnLoadingChanged;

            await base.SetParametersAsync(parameters);

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
