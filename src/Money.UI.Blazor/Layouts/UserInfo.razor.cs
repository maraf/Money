using Microsoft.AspNetCore.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Queries;
using Money.Services;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Layouts
{
    public abstract class UserInfoModel : ComponentBase, IDisposable, IEventHandler<UserSignedIn>, IEventHandler<UserSignedOut>
    {
        [Inject]
        public IQueryDispatcher Queries { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        internal Navigator Navigator { get; set; }

        [Inject]
        public ApiClient ApiClient { get; set; }

        protected ProfileModel Profile { get; private set; }
        protected bool IsAuthenticated => Profile != null;

        protected LoadingContext Loading { get; } = new LoadingContext();

        protected async override Task OnInitializedAsync()
        {
            BindEvents();

            await LoadProfileAsync();
        }

        private async Task LoadProfileAsync()
        {
            using (Loading.Start())
                Profile = await Queries.QueryAsync(new GetProfile());
        }

        protected async Task OnLogoutAsync()
        {
            await ApiClient.LogoutAsync();
            Navigator.OpenLogin();
        }

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
