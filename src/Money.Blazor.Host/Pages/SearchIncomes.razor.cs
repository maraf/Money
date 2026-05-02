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

public partial class SearchIncomes(
    Navigator Navigator,
    IEventHandlerCollection EventHandlers,
    IQueryDispatcher Queries,
    ILog<SearchIncomes> Log
) : 
    System.IDisposable,
    IEventHandler<IncomeCreated>,
    IEventHandler<IncomeDeleted>,
    IEventHandler<IncomeAmountChanged>,
    IEventHandler<IncomeDescriptionChanged>,
    IEventHandler<IncomeWhenChanged>,
    IEventHandler<PulledToRefresh>
{
    [Parameter]
    public string Query { get; set; }

    protected ElementReference SearchBox { get; set; }

    protected SortDescriptor<IncomeOverviewSortType> DefaultSort { get; set; }

    protected LoadingContext Loading { get; } = new LoadingContext();
    protected SortDescriptor<IncomeOverviewSortType> Sort { get; set; }
    protected PagingContext PagingContext { get; set; }

    protected List<IncomeOverviewModel> Models { get; set; } = [];
    protected string FormText { get; set; }
    protected SortDescriptor<IncomeOverviewSortType> FormSort { get; set; }

    protected async override Task OnInitializedAsync()
    {
        DefaultSort = await Queries.QueryAsync(new GetSearchIncomesSortProperty());
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

        if (SortDescriptor.TryParseFromUrl<IncomeOverviewSortType>(Navigator.FindQueryParameter("sort"), out var descriptor))
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
                var models = await Queries.QueryAsync(Money.Models.Queries.SearchIncomes.Version1(FormText, Sort, PagingContext.CurrentPageIndex));
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

    protected IncomeOverviewModel FindModel(IEvent payload)
        => Models.FirstOrDefault(o => o.Key.Equals(payload.AggregateKey));

    #region Events

    private void BindEvents()
    {
        EventHandlers
            .Add<IncomeCreated>(this)
            .Add<IncomeDeleted>(this)
            .Add<IncomeAmountChanged>(this)
            .Add<IncomeDescriptionChanged>(this)
            .Add<IncomeWhenChanged>(this)
            .Add<PulledToRefresh>(this);
    }

    private void UnBindEvents()
    {
        EventHandlers
            .Remove<IncomeCreated>(this)
            .Remove<IncomeDeleted>(this)
            .Remove<IncomeAmountChanged>(this)
            .Remove<IncomeDescriptionChanged>(this)
            .Remove<IncomeWhenChanged>(this)
            .Remove<PulledToRefresh>(this);
    }

    private Task UpdateModel(IEvent payload, Action<IncomeOverviewModel> handler)
    {
        IncomeOverviewModel model = FindModel(payload);
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

    Task IEventHandler<IncomeCreated>.HandleAsync(IncomeCreated payload)
        => ReloadDataAsync();

    Task IEventHandler<IncomeDeleted>.HandleAsync(IncomeDeleted payload)
        => ReloadDataAsync();

    Task IEventHandler<IncomeAmountChanged>.HandleAsync(IncomeAmountChanged payload)
    {
        if (Sort.Type == IncomeOverviewSortType.ByAmount)
            return ReloadDataAsync();
        else
            return UpdateModel(payload, model => model.Amount = payload.NewValue);
    }

    Task IEventHandler<IncomeDescriptionChanged>.HandleAsync(IncomeDescriptionChanged payload)
        => ReloadDataAsync();

    Task IEventHandler<IncomeWhenChanged>.HandleAsync(IncomeWhenChanged payload)
    {
        if (Sort.Type == IncomeOverviewSortType.ByWhen)
            return ReloadDataAsync();
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
