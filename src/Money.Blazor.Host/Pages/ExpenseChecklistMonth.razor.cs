using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Queries;

namespace Money.Pages;

public partial class ExpenseChecklistMonth(
    IQueryDispatcher Queries,
    IEventHandlerCollection EventHandlers,
    Navigator Navigator
) : 
    ComponentBase, 
    System.IDisposable,
    IEventHandler<OutcomeCreated>,
    IEventHandler<OutcomeDeleted>,
    IEventHandler<OutcomeAmountChanged>,
    IEventHandler<OutcomeDescriptionChanged>,
    IEventHandler<OutcomeWhenChanged>,
    IEventHandler<PulledToRefresh>,
    IEventHandler<SwipedLeft>,
    IEventHandler<SwipedRight>
{
    [Parameter]
    public int Year { get; set; }

    [Parameter]
    public int Month { get; set; }

    protected MonthModel SelectedPeriod { get; set; }
    protected List<ExpenseChecklistModel> Models { get; set; } = new();
    protected LoadingContext Loading { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        EnsureSelectedPeriod();
        BindEvents();
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        SelectedPeriod = null;
        return base.SetParametersAsync(parameters);
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        EnsureSelectedPeriod();
        await LoadDataAsync();
    }

    private void EnsureSelectedPeriod()
    {
        if (SelectedPeriod == null)
            SelectedPeriod = new MonthModel(Year, Month);
    }

    private async Task LoadDataAsync()
    {
        using (Loading.Start())
        {
            var data = await Queries.QueryAsync(new ListMonthExpenseChecklist(SelectedPeriod));
            Models.Clear();
            Models.AddRange(data);
        }
    }

    private Task ReloadDataAsync()
    {
        _ = LoadDataAsync().ContinueWith(t => StateHasChanged());
        return Task.CompletedTask;
    }

    public void Dispose()
        => UnBindEvents();

    #region Events

    private void BindEvents()
    {
        EventHandlers
            .Add<OutcomeCreated>(this)
            .Add<OutcomeDeleted>(this)
            .Add<OutcomeAmountChanged>(this)
            .Add<OutcomeDescriptionChanged>(this)
            .Add<OutcomeWhenChanged>(this)
            .Add<PulledToRefresh>(this)
            .Add<SwipedLeft>(this)
            .Add<SwipedRight>(this);
    }

    private void UnBindEvents()
    {
        EventHandlers
            .Remove<OutcomeCreated>(this)
            .Remove<OutcomeDeleted>(this)
            .Remove<OutcomeAmountChanged>(this)
            .Remove<OutcomeDescriptionChanged>(this)
            .Remove<OutcomeWhenChanged>(this)
            .Remove<PulledToRefresh>(this)
            .Remove<SwipedLeft>(this)
            .Remove<SwipedRight>(this);
    }

    Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload) => ReloadDataAsync();
    Task IEventHandler<OutcomeDeleted>.HandleAsync(OutcomeDeleted payload) => ReloadDataAsync();
    Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload) => ReloadDataAsync();
    Task IEventHandler<OutcomeDescriptionChanged>.HandleAsync(OutcomeDescriptionChanged payload) => ReloadDataAsync();
    Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload) => ReloadDataAsync();
    Task IEventHandler<PulledToRefresh>.HandleAsync(PulledToRefresh payload)
    {
        payload.IsHandled = true;
        return ReloadDataAsync();
    }

    Task IEventHandler<SwipedLeft>.HandleAsync(SwipedLeft payload)
    {
        Navigator.OpenChecklist(SelectedPeriod - 1);
        return Task.CompletedTask;
    }

    Task IEventHandler<SwipedRight>.HandleAsync(SwipedRight payload)
    {
        Navigator.OpenChecklist(SelectedPeriod + 1);
        return Task.CompletedTask;
    }

    #endregion
}