using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Layouts
{
    public partial class UserInfo : IDisposable, IEventHandler<UserSignedIn>, IEventHandler<UserSignedOut>
    {
        [Inject]
        public IQueryDispatcher Queries { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        internal Navigator Navigator { get; set; }

        [Inject]
        public ApiClient ApiClient { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthenticationState { get; set; }

        [Inject]
        public ILog<UserInfo> Log { get; set; }

        [Parameter]
        public string ListCssClass { get; set; }

        protected ProfileModel Profile { get; private set; }
        protected bool IsAuthenticated => Profile != null;

        protected LoadingContext Loading { get; } = new LoadingContext();

        protected async override Task OnInitializedAsync()
        {
            BindEvents();

            await base.OnInitializedAsync();
            await LoadProfileAsync();
        }

        private async Task LoadProfileAsync()
        {
            var state = await AuthenticationState.GetAuthenticationStateAsync();
            if (state.User.Identity.IsAuthenticated)
            {
                using (Loading.Start())
                    Profile = await Queries.QueryAsync(new GetProfile());
            }
        }

        protected Task OnLogoutAsync()
            => ApiClient.LogoutAsync();

        public void Dispose()
        {
            UnBindEvents();
        }

        #region Events

        private void BindEvents()
        {
            EventHandlers
                .Add<UserSignedIn>(this)
                .Add<UserSignedOut>(this);
        }

        private void UnBindEvents()
        {
            EventHandlers
                .Remove<UserSignedIn>(this)
                .Remove<UserSignedOut>(this);
        }

        async Task IEventHandler<UserSignedIn>.HandleAsync(UserSignedIn payload)
        {
            Log.Debug("User signed in.");

            await LoadProfileAsync();
            StateHasChanged();
        }

        Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
        {
            Profile = null;
            StateHasChanged();
            return Task.CompletedTask;
        }

        #endregion
    }
}
