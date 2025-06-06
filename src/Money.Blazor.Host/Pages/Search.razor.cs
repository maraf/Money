﻿using Microsoft.AspNetCore.Components;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Queries;
using Money.Services;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Money.Pages;

public partial class Search(
    Navigator Navigator,
    IEventHandlerCollection EventHandlers,
    IQueryDispatcher Queries,
    ILog<Search> Log
) : 
    System.IDisposable,
    IEventHandler<OutcomeCreated>,
    IEventHandler<OutcomeDeleted>,
    IEventHandler<OutcomeAmountChanged>,
    IEventHandler<OutcomeDescriptionChanged>,
    IEventHandler<OutcomeWhenChanged>,
    IEventHandler<ExpenseExpectedWhenChanged>,
    IEventHandler<PulledToRefresh>
{
    [Parameter]
    public string Query { get; set; }

    protected ElementReference SearchBox { get; set; }

    protected SortDescriptor<OutcomeOverviewSortType> DefaultSort { get; set; }

    protected LoadingContext Loading { get; } = new LoadingContext();
    protected SortDescriptor<OutcomeOverviewSortType> Sort { get; set; }
    protected PagingContext PagingContext { get; set; }

    protected List<OutcomeOverviewModel> Models { get; set; } = [];
    protected string FormText { get; set; }
    protected SortDescriptor<OutcomeOverviewSortType> FormSort { get; set; }

    protected async override Task OnInitializedAsync()
    {
        DefaultSort = await Queries.QueryAsync(new GetSearchSortProperty());
        FormSort = Sort = DefaultSort;
        PagingContext = new PagingContext(LoadPageAsync, Loading);
        Navigator.LocationChanged += OnLocationChanged;
        BindEvents();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        await OnSearchAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
            await SearchBox.FocusAsync();
    }

    public void Dispose()
    {
        Navigator.LocationChanged -= OnLocationChanged;
        UnBindEvents();
    }

    private async void OnLocationChanged(string url)
    {
        await OnSearchAsync();
        StateHasChanged();
    }

    protected async Task OnSearchAsync()
    {
        string lastQuery = Query;
        var lastSort = Sort;
        FormText = Query = Navigator.FindQueryParameter("q");

        if (SortDescriptor.TryParseFromUrl<OutcomeOverviewSortType>(Navigator.FindQueryParameter("sort"), out var descriptor))
            FormSort = Sort = descriptor;
        else
            FormSort = Sort = DefaultSort;

        Log.Debug($"Sort: last '{lastSort.Type}+{lastSort.Direction}', current '{Sort.Type}+{Sort.Direction}'.");

        if (lastQuery == Query && lastSort.Equals(Sort))
        {
            Log.Debug("No change in query or sort.");
            return;
        }

        Log.Debug($"Load first page with '{Query}' and '{Sort}'.");
        await PagingContext.LoadAsync(0);
    }

    protected async Task<PagingLoadStatus> LoadPageAsync()
    {
        using (Loading.Start())
        {
            Log.Debug($"Starting to load {PagingContext.CurrentPageIndex}");
            if (PagingContext.CurrentPageIndex == 0)
            {
                Models = [];
                StateHasChanged();
            }

            if (!String.IsNullOrEmpty(FormText))
            {
                var models = await Queries.QueryAsync(SearchOutcomes.Version3(FormText, Sort, PagingContext.CurrentPageIndex));
                if (models.Count == 0)
                {
                    Log.Debug("Empty result");
                    return PagingLoadStatus.EmptyPage;
                }

                Models.AddRange(models);

                var result = models.Count == 10 ? PagingLoadStatus.HasNextPage : PagingLoadStatus.LastPage;
                Log.Debug($"Loading finished, all items '{Models.Count}', result '{result}'");
                StateHasChanged();
                return result;
            }
            else
            {
                Log.Debug($"Empty query");
                Models.Clear();
                return PagingLoadStatus.LastPage;
            }
        }
    }

    protected OutcomeOverviewModel FindModel(IEvent payload)
        => Models.FirstOrDefault(o => o.Key.Equals(payload.AggregateKey));

    #region Events

    private void BindEvents()
    {
        EventHandlers
            .Add<OutcomeCreated>(this)
            .Add<OutcomeDeleted>(this)
            .Add<OutcomeAmountChanged>(this)
            .Add<OutcomeDescriptionChanged>(this)
            .Add<OutcomeWhenChanged>(this)
            .Add<ExpenseExpectedWhenChanged>(this)
            .Add<PulledToRefresh>(this);
    }

    private void UnBindEvents()
    {
        EventHandlers
            .Remove<OutcomeCreated>(this)
            .Remove<OutcomeDeleted>(this)
            .Remove<OutcomeAmountChanged>(this)
            .Remove<OutcomeDescriptionChanged>(this)
            .Remove<OutcomeWhenChanged>(this)
            .Remove<ExpenseExpectedWhenChanged>(this)
            .Remove<PulledToRefresh>(this);
    }

    private Task UpdateModel(IEvent payload, Action<OutcomeOverviewModel> handler)
    {
        OutcomeOverviewModel model = FindModel(payload);
        if (model != null)
        {
            handler(model);
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    private Task ReloadDataAsync()
    {
        _ = PagingContext.LoadAsync(0).ContinueWith(_ => StateHasChanged());
        return Task.CompletedTask;
    }

    Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload)
        => ReloadDataAsync();

    Task IEventHandler<OutcomeDeleted>.HandleAsync(OutcomeDeleted payload)
        => ReloadDataAsync();

    Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload)
    {
        if (Sort.Type == OutcomeOverviewSortType.ByAmount)
            return ReloadDataAsync();
        else
            return UpdateModel(payload, model => model.Amount = payload.NewValue);
    }

    Task IEventHandler<OutcomeDescriptionChanged>.HandleAsync(OutcomeDescriptionChanged payload)
        => ReloadDataAsync();

    Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload)
    {
        if (Sort.Type == OutcomeOverviewSortType.ByWhen)
            return ReloadDataAsync();
        else
            return UpdateModel(payload, model => model.When = payload.When);
    }

    Task IEventHandler<ExpenseExpectedWhenChanged>.HandleAsync(ExpenseExpectedWhenChanged payload)
    {
        UpdateModel(payload, model => model.ExpectedWhen = payload.When);
        return Task.CompletedTask;
    }

    async Task IEventHandler<PulledToRefresh>.HandleAsync(PulledToRefresh payload)
    {
        payload.IsHandled = true;
        await LoadPageAsync();
        StateHasChanged();
    }

    #endregion
}
