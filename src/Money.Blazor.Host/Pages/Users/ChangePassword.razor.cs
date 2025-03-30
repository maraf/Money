using Microsoft.AspNetCore.Components;
using Money.Events;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages.Users;

public partial class ChangePassword(ICommandDispatcher Commands, IEventHandlerCollection EventHandlers) : IEventHandler<PasswordChanged>, IDisposable
{
    protected ElementReference CurrentPasswordBox { get; set; }

    public string Current { get; set; }
    public string New { get; set; }
    public string Confirm { get; set; }

    public bool IsSuccess { get; set; }

    protected override Task OnInitializedAsync()
    {
        BindEvents();
        return base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
            await CurrentPasswordBox.FocusAsync();
    }

    protected async Task OnFormSubmit()
    {
        IsSuccess = false;

        if (!String.IsNullOrEmpty(Current) && !String.IsNullOrEmpty(New) && New == Confirm)
            await Commands.HandleAsync(new Commands.ChangePassword(Current, New));
    }

    public void Dispose()
    {
        UnBindEvents();
    }

    #region Events

    private void BindEvents()
    {
        EventHandlers
            .Add<PasswordChanged>(this);
    }

    private void UnBindEvents()
    {
        EventHandlers
            .Remove<PasswordChanged>(this);
    }

    Task IEventHandler<PasswordChanged>.HandleAsync(PasswordChanged payload)
    {
        Current = null;
        New = null;
        Confirm = null;
        IsSuccess = true;
        StateHasChanged();
        return Task.CompletedTask;
    }

    #endregion
}
