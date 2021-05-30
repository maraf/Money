using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Neptuo.Logging;
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

        [Inject]
        protected ILog<AuthenticatedView> Log { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public Action<bool> OnChanged { get; set; }

        protected AuthenticationState AuthenticationState { get; set; }
        protected bool IsAuthenticated { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadAsync();
        }

        public void Dispose()
        {
            AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        private void OnAuthenticationStateChanged(Task<AuthenticationState> task) 
            => _ = LoadAsync();

        private async Task LoadAsync()
        {
            Log.Debug("Load.");

            AuthenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var newValue = AuthenticationState != null && AuthenticationState.User.Identity.IsAuthenticated;
            Log.Debug($"New value '{newValue}'.");

            if (IsAuthenticated != newValue)
            {
                Log.Debug($"Rendering child content.");

                IsAuthenticated = newValue;
                OnChanged?.Invoke(IsAuthenticated);
                StateHasChanged();
            }
        }
    }
}
