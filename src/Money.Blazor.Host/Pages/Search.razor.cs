using Microsoft.AspNetCore.Components;
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
        PagingContext = new PagingContext(() => LoadPageAsync(), Loading);
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
            return;

        var status = await PagingContext.LoadAsync(0);
        if (status == PagingLoadStatus.EmptyPage && lastQuery != Query)
            Models.Clear();
    }

    protected async Task<PagingLoadStatus> LoadPageAsync(bool cleanOnEmptyResults = false)
    {
        if (!String.IsNullOrEmpty(FormText))
        {
            var models = await Queries.QueryAsync(SearchOutcomes.Version2(FormText, Sort, PagingContext.CurrentPageIndex));
            if (models.Count == 0)
            {
                if (cleanOnEmptyResults)
                    Models.Clear();

                return PagingLoadStatus.EmptyPage;
            }

            Models = models;
            return Models.Count == 10 ? PagingLoadStatus.HasNextPage : PagingLoadStatus.LastPage;
        }
        else
        {
            Models.Clear();
            return PagingLoadStatus.LastPage;
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

    private Task ReloadPageAsync()
    {
        _ = LoadPageAsync(cleanOnEmptyResults: true).ContinueWith(_ => StateHasChanged());
        return Task.CompletedTask;
    }

    Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload)
        => ReloadPageAsync();

    Task IEventHandler<OutcomeDeleted>.HandleAsync(OutcomeDeleted payload)
        => ReloadPageAsync();

    Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload)
    {
        if (Sort.Type == OutcomeOverviewSortType.ByAmount)
            return ReloadPageAsync();
        else
            return UpdateModel(payload, model => model.Amount = payload.NewValue);
    }

    Task IEventHandler<OutcomeDescriptionChanged>.HandleAsync(OutcomeDescriptionChanged payload)
        => ReloadPageAsync();

    Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload)
    {
        if (Sort.Type == OutcomeOverviewSortType.ByWhen)
            return ReloadPageAsync();
        else
            return UpdateModel(payload, model => model.When = payload.When);
    }

    async Task IEventHandler<PulledToRefresh>.HandleAsync(PulledToRefresh payload)
    {
        payload.IsHandled = true;
        await LoadPageAsync();
        StateHasChanged();
    }

    #endregion
}
