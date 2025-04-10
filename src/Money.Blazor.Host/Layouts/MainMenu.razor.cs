using Microsoft.AspNetCore.Components;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Money.Layouts;

public interface IMainMenu
{
    bool IsMainMenuVisible { get; }
    void UpdateMainMenuVisible(bool isVisible);
}

public partial class MainMenu(
    Navigator Navigator,
    IQueryDispatcher Queries,
    ILog<MainMenu> Log,
    IEventHandlerCollection EventHandlers,
    Navigator.ComponentContainer ComponentContainer
) : 
    IDisposable,
    IMainMenu,
    IEventHandler<UserSignedOut>
{
    public bool IsMainMenuVisible { get; protected set; } = false;

    protected List<MenuItemModel> ViewsItems { get; set; }
    protected List<MenuItemModel> MoreItems { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ComponentContainer.MainMenu = this;

        Navigator.LocationChanged += OnLocationChanged;
        EventHandlers.Add<UserSignedOut>(this);
    }

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var items = await Queries.QueryAsync(new ListMainMenuItem());
        ViewsItems = items.Views;
        MoreItems = items.More;
    }

    public void Dispose()
    {
        Navigator.LocationChanged -= OnLocationChanged;
        EventHandlers.Remove<UserSignedOut>(this);
    }

    public void UpdateMainMenuVisible(bool isVisible)
    {
        if (IsMainMenuVisible != isVisible)
        {
            Log.Debug($"IsMainMenuVisible: Previous='{IsMainMenuVisible}', New='{isVisible}'.");
            IsMainMenuVisible = isVisible;
            StateHasChanged();
        }
    }

    Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
    {
        UpdateMainMenuVisible(false);
        return Task.CompletedTask;
    }

    private void OnLocationChanged(string url)
        => UpdateMainMenuVisible(false);
}
