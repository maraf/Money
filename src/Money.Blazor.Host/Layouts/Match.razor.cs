using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Layouts
{
    public partial class Match : IDisposable
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string Url { get; set; }

        [Parameter]
        public MatchMode Mode { get; set; } = MatchMode.Exact;

        [Parameter]
        public Type PageType { get; set; }

        [Parameter]
        public RenderFragment<bool> ChildContent { get; set; }

        [CascadingParameter]
        public RouteData RouteData { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
            => StateHasChanged();

        protected bool IsActive()
        {
            if (PageType != null && RouteData != null)
                return PageType == RouteData.PageType;

            string currentUrl = "/" + NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            if ((Mode == MatchMode.Exact && Url == currentUrl) || currentUrl.StartsWith(Url))
                return true;

            return false;
        }
    }
}
