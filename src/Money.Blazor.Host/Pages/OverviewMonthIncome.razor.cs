﻿using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Money.Pages;

public partial class OverviewMonthIncome(
    ICommandDispatcher Commands,
    IEventHandlerCollection EventHandlers,
    IQueryDispatcher Queries,
    Interop Interop,
    Navigator Navigator
) : 
    IDisposable,
    IEventHandler<IncomeCreated>,
    IEventHandler<IncomeAmountChanged>,
    IEventHandler<IncomeDescriptionChanged>,
    IEventHandler<IncomeWhenChanged>,
    IEventHandler<IncomeDeleted>,
    IEventHandler<PulledToRefresh>,
    IEventHandler<SwipedLeft>,
    IEventHandler<SwipedRight>
{
    [Parameter]
    public int Year { get; set; }

    [Parameter]
    public int Month { get; set; }

    protected MonthModel SelectedPeriod { get; set; }

    protected List<IncomeOverviewModel> Items { get; set; }
    protected IncomeOverviewModel SelectedItem { get; set; }

    protected IncomeCreate CreateModal { get; set; }

    protected LoadingContext Loading { get; } = new LoadingContext();
    protected SortDescriptor<IncomeOverviewSortType> SortDescriptor { get; set; } = new(IncomeOverviewSortType.ByWhen, SortDirection.Descending);
    protected PagingContext PagingContext { get; set; }

    protected IncomeOverviewModel Selected { get; set; }
    protected string DeleteMessage { get; set; }
    protected Confirm DeleteConfirm { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        SelectedPeriod = new MonthModel(Year, Month);
        PagingContext = new PagingContext(LoadDataAsync, Loading);

        BindEvents();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        SelectedPeriod = new MonthModel(Year, Month);
        Reload();
    }

    protected async void Reload()
    {
        await PagingContext.LoadAsync(0);
        StateHasChanged();
    }

    protected async Task<PagingLoadStatus> LoadDataAsync()
    {
        await Interop.ScrollToTopAsync();

        List<IncomeOverviewModel> models = await Queries.QueryAsync(new ListMonthIncome(SelectedPeriod, SortDescriptor, PagingContext.CurrentPageIndex));
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
        await PagingContext.LoadAsync(0);
        StateHasChanged();
    }

    protected void OnActionClick(IncomeOverviewModel model, ModalDialog modal)
    {
        SelectedItem = model;
        modal.Show();
        StateHasChanged();
    }

    protected void OnDeleteClick(IncomeOverviewModel model)
    {
        Selected = model;
        DeleteMessage = $"Do you really want to delete income '{model.Description}'?";
        DeleteConfirm.Show();
        StateHasChanged();
    }

    protected void Edit(IncomeOverviewModel model, ModalDialog modal)
    {
        Selected = model;
        modal.Show();
        StateHasChanged();
    }

    protected async void OnDeleteConfirmed()
    {
        await Commands.HandleAsync(new DeleteIncome(Selected.Key));
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
        if (SelectedPeriod == payload.When)
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
        Navigator.OpenOverviewIncomes(SelectedPeriod - 1);
        return Task.CompletedTask;
    }

    Task IEventHandler<SwipedRight>.HandleAsync(SwipedRight payload)
    {
        Navigator.OpenOverviewIncomes(SelectedPeriod + 1);
        return Task.CompletedTask;
    }

    #endregion
}

