using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Pages;
using Money.Services;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Layouts
{
    public partial class BottomMenu : IDisposable,
        IEventHandler<UserPropertyChanged>
    {
        [Inject]
        protected Navigator Navigator { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected List<IActionMenuItemModel> Items { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            EventHandlers.Add<UserPropertyChanged>(this);
            AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadAsync();
        }

        public void Dispose()
        {
            EventHandlers.Remove<UserPropertyChanged>(this);
        }

        private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            await LoadAsync();
            StateHasChanged();
        }

        private async Task LoadAsync()
        {
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (state.User.Identity.IsAuthenticated)
                Items = await Queries.QueryAsync(new ListBottomMenuItem());
            else
                Items = null;
        }

        async Task IEventHandler<UserPropertyChanged>.HandleAsync(UserPropertyChanged payload)
        {
            await LoadAsync();
            StateHasChanged();
        }
    }
}
