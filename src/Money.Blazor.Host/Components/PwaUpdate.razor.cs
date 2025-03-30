using Microsoft.AspNetCore.Components;
using Money.Events;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public partial class PwaUpdate(
    IEventHandlerCollection EventHandlers,
    ICommandDispatcher Commands,
    Navigator Navigator
) : IDisposable,
    IEventHandler<PwaUpdateable>
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    protected bool IsUpdateable { get; set; }
    protected MarkupString ReleaseNotes { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        EventHandlers.Add<PwaUpdateable>(this);
    }

    public void Dispose()
    {
        EventHandlers.Remove<PwaUpdateable>(this);
    }

    Task IEventHandler<PwaUpdateable>.HandleAsync(PwaUpdateable payload)
    {
#if !DEBUG
        IsUpdateable = true;
        StateHasChanged();
#endif
        return Task.CompletedTask;
    }
}
