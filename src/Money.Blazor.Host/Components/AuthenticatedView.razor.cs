using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class AuthenticatedView : IDisposable
    {
        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected AuthenticationState AuthenticationState { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            AuthenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        }

        public void Dispose()
        {
            AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            AuthenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            StateHasChanged();
        }
    }
}
