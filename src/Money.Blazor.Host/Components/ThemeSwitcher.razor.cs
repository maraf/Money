using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Money.Events;
using Money.Queries;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Queries;

namespace Money.Components;

public partial class ThemeSwitcher : IEventHandler<UserSignedIn>, IEventHandler<UserPropertyChanged>, System.IDisposable
{
    [Inject]
    protected IEventHandlerCollection EventHandlers { get; set; }

    [Inject]
    protected IQueryDispatcher Queries { get; set; }

    [Inject]
    protected Interop Interop { get; set; }

    protected ThemeType Theme { get; set; }

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        EventHandlers
            .Add<UserSignedIn>(this)
            .Add<UserPropertyChanged>(this);

        await ApplyThemeAsync();
    }

    private async Task ApplyThemeAsync()
    {
        Theme = await Queries.QueryAsync(new GetThemeTypeProperty());
        await Interop.ApplyThemeAsync(Theme switch {
            ThemeType.Light => "light",
            ThemeType.Dark => "dark",
            _ => throw new NotSupportedException($"Missing case for '{Theme}'")
        });
    }

    public void Dispose()
    {
        EventHandlers
            .Remove<UserSignedIn>(this)
            .Remove<UserPropertyChanged>(this);
    }

    async Task IEventHandler<UserSignedIn>.HandleAsync(UserSignedIn payload)
    {
        await ApplyThemeAsync();
        StateHasChanged();
    }

    async Task IEventHandler<UserPropertyChanged>.HandleAsync(UserPropertyChanged payload)
    {
        await ApplyThemeAsync();
        StateHasChanged();
    }
}