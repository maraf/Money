using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Money.Layouts;

public partial class UserInfo(
    IQueryDispatcher Queries,
    IEventHandlerCollection EventHandlers,
    AuthenticationStateProvider AuthenticationState,
    ILog<UserInfo> Log
) : 
    IDisposable, 
    IEventHandler<UserSignedIn>, 
    IEventHandler<UserSignedOut>
{
    [Parameter]
    public string ListCssClass { get; set; }

    protected ProfileModel Profile { get; private set; }
    protected bool IsAuthenticated => Profile != null;

    protected LoadingContext Loading { get; } = new LoadingContext();

    protected List<MenuItemModel> MenuItems { get; set; }

    protected async override Task OnInitializedAsync()
    {
        BindEvents();

        await base.OnInitializedAsync();
        await LoadProfileAsync();

        MenuItems = (await Queries.QueryAsync(new ListMainMenuItem())).User;
    }

    private async Task LoadProfileAsync()
    {
        var state = await AuthenticationState.GetAuthenticationStateAsync();
        if (state.User.Identity.IsAuthenticated)
        {
            Log.Debug("User is authenticated");

            using (Loading.Start())
                Profile = await Queries.QueryAsync(new GetProfile());

            Log.Debug($"User is '{Profile?.UserName}'");
        }
        else
        {
            Log.Debug("User is NOT authenticated");
        }
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
