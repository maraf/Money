
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Events;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Models;
using Neptuo.Models.Keys;

namespace Money.Components;

partial class CommandExecutionDecorator : ComponentBase, System.IDisposable, IEventHandler<object>
{
    [Inject]
    protected IEventHandlerCollection EventHandlers { get; set; }

    [Parameter]
    public IKey AggregateKey { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    private int pendingOperations;
    
    public bool IsExecuting => pendingOperations > 0;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        EventHandlers.Add<object>(this);
    }

    public void Dispose()
    {
        EventHandlers.Remove<object>(this);
    }

    public Task HandleAsync(object payload)
    {
        if (payload is HttpCommandSending httpCommand)
        {
            if (httpCommand.Command is IAggregateCommand command && AggregateKey.Equals(command.AggregateKey))
            {
                pendingOperations++;
                StateHasChanged();
            }
        }
        else if (payload is Event e)
        {
            if (AggregateKey.Equals(e.AggregateKey))
            {
                pendingOperations--;
                StateHasChanged();
            }
        }
        else if (payload is ExceptionRaised exception)
        {
            if (exception.Exception is AggregateRootException ex && AggregateKey.Equals(ex.Key))
            {
                pendingOperations--;
                StateHasChanged();
            }
        }

        return Task.CompletedTask;
    }
}