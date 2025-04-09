using Money.Components.Bootstrap;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Money.Layouts;

public partial class BottomMenu(
    Navigator Navigator,
    IQueryDispatcher Queries,
    IEventHandlerCollection EventHandlers,
    Interop Interop
) : 
    IDisposable,
    IEventHandler<UserPropertyChanged>
{
    protected List<IActionMenuItemModel> Items { get; set; }
    protected MainMenuItems MainMenu { get; set; }
    protected Offcanvas Offcanvas { get; set; }

    protected async override Task OnInitializedAsync()
    {
        EventHandlers.Add<UserPropertyChanged>(this);

        Navigator.LocationChanged += OnLocationChanged;

        await base.OnInitializedAsync();
        await LoadAsync();
        MainMenu = await Queries.QueryAsync(new ListMainMenuItem());
    }

    public void Dispose()
    {
        Navigator.LocationChanged -= OnLocationChanged;
        EventHandlers.Remove<UserPropertyChanged>(this);
    }

    private async Task LoadAsync()
    {
        Items = await Queries.QueryAsync(new ListBottomMenuItem());
    }

    async Task IEventHandler<UserPropertyChanged>.HandleAsync(UserPropertyChanged payload)
    {
        await LoadAsync();
        StateHasChanged();
    }

    protected async Task OnLinkClick(bool isBlurMenuAfterClick)
    {
        if (isBlurMenuAfterClick)
            await Interop.BlurActiveElementAsync();
    }

    protected void OnToggleMainMenu()
    {
        if (Offcanvas == null)
            return;

        if (Offcanvas.IsVisible)
            Offcanvas.Hide();
        else
            Offcanvas.Show();
    }

    private void OnLocationChanged(string url)
        => Offcanvas?.Hide();
}
