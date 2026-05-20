using Microsoft.AspNetCore.Components;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Services;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Money.Pages;

public partial class OverviewIncome<T>(
    IEventHandlerCollection EventHandlers,
    IQueryDispatcher Queries,
    Interop Interop,
    Navigator Navigator,
    string subTitle = null
) :
    System.IDisposable,
    IEventHandler<IncomeCreated>,
    IEventHandler<IncomeAmountChanged>,
    IEventHandler<IncomeDescriptionChanged>,
    IEventHandler<IncomeWhenChanged>,
    IEventHandler<IncomeDeleted>,
    IEventHandler<PulledToRefresh>,
    IEventHandler<SwipedLeft>,
    IEventHandler<SwipedRight>
{
    protected Navigator Navigator { get; } = Navigator;
    protected string SubTitle { get; set; } = subTitle;

    protected T SelectedPeriod { get; set; }
    protected List<IncomeOverviewModel> Items { get; set; }

    protected IncomeCreate CreateModal { get; set; }

    protected LoadingContext Loading { get; } = new LoadingContext();
    protected SortDescriptor<IncomeOverviewSortType> SortDescriptor { get; set; } = new(IncomeOverviewSortType.ByWhen, SortDirection.Descending);
    protected PagingContext PagingContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        SelectedPeriod = CreateSelectedItemFromParameters();
        PagingContext = new PagingContext(LoadDataAsync, Loading);

        BindEvents();
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        ClearPreviousParameters();
        return base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        SelectedPeriod = CreateSelectedItemFromParameters();
        Reload();
    }

    protected virtual void ClearPreviousParameters()
        => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(ClearPreviousParameters)}'.");

    protected virtual T CreateSelectedItemFromParameters()
        => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateSelectedItemFromParameters)}'.");

    protected virtual IQuery<List<IncomeOverviewModel>> CreateItemsQuery(int pageIndex)
        => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateItemsQuery)}'.");

    protected virtual string ListExpenseUrl()
        => null;

    protected virtual string ChecklistUrl()
        => null;

    protected virtual bool IsContained(DateTime when)
        => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(IsContained)}'.");

    protected virtual void OpenNextPeriod()
    { }

    protected virtual void OpenPrevPeriod()
    { }

    protected async void Reload()
    {
        Items = null;
        StateHasChanged();
        await PagingContext.LoadAsync(0);
        StateHasChanged();
    }

    protected async Task<PagingLoadStatus> LoadDataAsync()
    {
        await Interop.ScrollToTopAsync();

        List<IncomeOverviewModel> models = await Queries.QueryAsync(CreateItemsQuery(PagingContext.CurrentPageIndex));
        if (models.Count == 0)
        {
            if (PagingContext.CurrentPageIndex == 0)
                Items = models;

            return PagingLoadStatus.EmptyPage;
        }

        Items = models;
        return Items.Count == 10 ? PagingLoadStatus.HasNextPage : PagingLoadStatus.LastPage;
    }

    protected async void OnSortChanged()
    {
        Items = null;
        StateHasChanged();
        await PagingContext.LoadAsync(0);
        StateHasChanged();
    }

    protected IncomeOverviewModel FindModel(IEvent payload)
        => Items.FirstOrDefault(o => o.Key.Equals(payload.AggregateKey));

    public void Dispose()
        => UnBindEvents();

    #region Events

    private void BindEvents()
    {
        EventHandlers
            .Add<IncomeCreated>(this)
            .Add<IncomeAmountChanged>(this)
            .Add<IncomeWhenChanged>(this)
            .Add<IncomeDescriptionChanged>(this)
            .Add<IncomeDeleted>(this)
            .Add<PulledToRefresh>(this)
            .Add<SwipedLeft>(this)
            .Add<SwipedRight>(this);
    }

    private void UnBindEvents()
    {
        EventHandlers
            .Remove<IncomeCreated>(this)
            .Remove<IncomeAmountChanged>(this)
            .Remove<IncomeWhenChanged>(this)
            .Remove<IncomeDescriptionChanged>(this)
            .Remove<IncomeDeleted>(this)
            .Remove<PulledToRefresh>(this)
            .Remove<SwipedLeft>(this)
            .Remove<SwipedRight>(this);
    }

    protected Task OnEventAsync(IKey aggregateKey, Action<IncomeOverviewModel> handler)
    {
        var item = Items.FirstOrDefault(i => i.Key.Equals(aggregateKey));
        if (item != null)
            handler(item);

        StateHasChanged();
        return Task.CompletedTask;
    }

    Task IEventHandler<IncomeCreated>.HandleAsync(IncomeCreated payload)
    {
        if (IsContained(payload.When))
            Reload();

        return Task.CompletedTask;
    }

    Task IEventHandler<IncomeDeleted>.HandleAsync(IncomeDeleted payload)
        => OnEventAsync(payload.AggregateKey, item => Items.Remove(item));

    Task IEventHandler<IncomeWhenChanged>.HandleAsync(IncomeWhenChanged payload)
    {
        Reload();
        return Task.CompletedTask;
    }

    Task IEventHandler<IncomeDescriptionChanged>.HandleAsync(IncomeDescriptionChanged payload)
        => OnEventAsync(payload.AggregateKey, item => item.Description = payload.Description);

    Task IEventHandler<IncomeAmountChanged>.HandleAsync(IncomeAmountChanged payload)
        => OnEventAsync(payload.AggregateKey, item => item.Amount = payload.NewValue);

    async Task IEventHandler<PulledToRefresh>.HandleAsync(PulledToRefresh payload)
    {
        payload.IsHandled = true;
        await LoadDataAsync();
        StateHasChanged();
    }

    Task IEventHandler<SwipedLeft>.HandleAsync(SwipedLeft payload)
    {
        OpenPrevPeriod();
        return Task.CompletedTask;
    }

    Task IEventHandler<SwipedRight>.HandleAsync(SwipedRight payload)
    {
        OpenNextPeriod();
        return Task.CompletedTask;
    }

    #endregion
}
