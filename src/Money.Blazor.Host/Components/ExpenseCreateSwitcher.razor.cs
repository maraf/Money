using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Queries;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public partial class ExpenseCreateSwitcher(
    ILog<ExpenseCreateSwitcher> Log,
    IQueryDispatcher Queries,
    IEventHandlerCollection EventHandlers
) : IEventHandler<UserPropertyChanged>, System.IDisposable
{
    protected ExpenseCreateDialogType Selected { get; set; }

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ReloadAsync();

        EventHandlers.Add<UserPropertyChanged>(this);
    }

    public void Dispose()
    {
        EventHandlers.Remove<UserPropertyChanged>(this);
    }

    private async Task ReloadAsync()
    {
        Selected = await Queries.QueryAsync(new GetExpenseCreateDialogTypeProperty());
        Log.Debug($"User prefered ExpenseCreateDialog is '{Selected}'");
    }

    async Task IEventHandler<UserPropertyChanged>.HandleAsync(UserPropertyChanged payload)
    {
        Log.Debug($"Got UserPropertyChanged event for propertyKey '{payload.PropertyKey}' with value '{payload.Value}'");
        if (payload.PropertyKey == GetExpenseCreateDialogTypeProperty.PropertyKey)
        {
            await ReloadAsync();
            StateHasChanged();
        }
    }
}