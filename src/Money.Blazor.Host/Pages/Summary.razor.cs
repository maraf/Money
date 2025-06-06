﻿using Microsoft.AspNetCore.Components;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Services;
using Money.Queries;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Money.Pages;

public partial class Summary<T>(
    IEventHandlerCollection EventHandlers,
    IQueryDispatcher Queries,
    ILog<Summary<T>> Log,
    Navigator Navigator,
    string subTitle = null
) : 
    System.IDisposable,
    IEventHandler<OutcomeCreated>,
    IEventHandler<OutcomeDeleted>,
    IEventHandler<OutcomeAmountChanged>,
    IEventHandler<OutcomeWhenChanged>,
    IEventHandler<PulledToRefresh>,
    IEventHandler<SwipedLeft>,
    IEventHandler<SwipedRight>,
    IEventHandler<IncomeCreated>,
    IEventHandler<IncomeDeleted>
{
    protected string SubTitle { get; set; } = subTitle;

    protected bool IsPeriodReloadRequired { get; set; }
    protected List<T> Periods { get; private set; }
    protected T SelectedPeriod { get; set; }
    protected IReadOnlyCollection<T> PeriodGuesses { get; set; }

    protected LoadingContext CategoriesLoading { get; } = new LoadingContext();
    protected Price IncomeTotal { get; private set; }
    protected Price ExistingExpenseTotal { get; private set; }
    protected Price ExpectedExpenseTotal { get; private set; }
    protected string DefaultCurrency { get; private set; }
    protected SummaryDisplayType SelectedDisplayType { get; set; }
    protected List<CategoryWithAmountModel> Categories { get; private set; }
    protected SortDescriptor<SummarySortType> SortDescriptor { get; set; }

    protected IncomeCreate IncomeCreate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Log.Debug("Summary.OnInitializedAsync");
        BindEvents();

        await base.OnInitializedAsync();

        DefaultCurrency = await Queries.QueryAsync(new FindCurrencyDefault());
        SelectedDisplayType = await Queries.QueryAsync(new GetSummaryDisplayProperty());
        SortDescriptor = await Queries.QueryAsync(new GetSummarySortProperty());
    }

    protected virtual void ClearPreviousParameters()
        => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(ClearPreviousParameters)}'.");

    protected virtual (T, IReadOnlyCollection<T>) CreateSelectedPeriodFromParameters()
        => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateSelectedPeriodFromParameters)}'.");

    public override Task SetParametersAsync(ParameterView parameters)
    {
        ClearPreviousParameters();
        return base.SetParametersAsync(parameters);
    }

    protected async override Task OnParametersSetAsync()
    {
        (SelectedPeriod, PeriodGuesses) = CreateSelectedPeriodFromParameters();
        await LoadSelectedPeriodAsync();
    }

    protected virtual IQuery<List<T>> CreatePeriodsQuery()
        => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreatePeriodsQuery)}'.");

    protected async Task<IReadOnlyCollection<T>> GetPeriodsAsync()
    {
        if (IsPeriodReloadRequired || Periods == null)
        {
            IsPeriodReloadRequired = false;
            Periods = await Queries.QueryAsync(CreatePeriodsQuery());
        }

        return Periods;
    }

    protected async Task LoadSelectedPeriodAsync()
    {
        if (SelectedPeriod != null)
        {
            using (CategoriesLoading.Start())
            {
                try
                {
                    Categories = await Queries.QueryAsync(CreateCategoriesQuery(SelectedPeriod));

                    await LoadIncomeTotalAsync();
                    await LoadExpectedExpenseTotalAsync();
                    ExistingExpenseTotal = await LoadTotalAsync(CreateExistingExpenseTotalQuery);
                }
                catch (MissingDefaultCurrentException)
                {
                    Navigator.OpenCurrencies();
                }
            }

            Sort();
        }
    }

    private async Task LoadIncomeTotalAsync() 
        => IncomeTotal = await LoadTotalAsync(CreateIncomeTotalQuery);

    private async Task LoadExpectedExpenseTotalAsync()
        => ExpectedExpenseTotal = await LoadTotalAsync(CreateExpectedExpenseTotalQuery);

    private async Task<Price> LoadTotalAsync(Func<T, IQuery<Price>> queryFactory)
    {
        var query = queryFactory(SelectedPeriod);
        if (query != null)
            return await Queries.QueryAsync(query);

        return null;
    }

    protected virtual IQuery<Price> CreateIncomeTotalQuery(T item)
        => null;

    protected virtual IQuery<Price> CreateExistingExpenseTotalQuery(T item)
        => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateExistingExpenseTotalQuery)}'.");

    protected virtual IQuery<Price> CreateExpectedExpenseTotalQuery(T item)
        => null;

    protected virtual IQuery<List<CategoryWithAmountModel>> CreateCategoriesQuery(T item)
        => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateCategoriesQuery)}'.");

    protected void OnSorted()
    {
        Sort();
        StateHasChanged();
    }

    private void Sort()
    {
        Log.Debug($"Sorting: Type='{SortDescriptor.Type}', Direction='{SortDescriptor.Direction}'.");

        switch (SortDescriptor.Type)
        {
            case SummarySortType.ByAmount:
                Categories.Sort(SortDescriptor.Direction, x => x.TotalAmount.Value);
                break;
            case SummarySortType.ByCategory:
                Categories.Sort(SortDescriptor.Direction, x => x.Name);
                break;
            default:
                throw Ensure.Exception.NotSupported(SortDescriptor.Type.ToString());
        }
    }

    protected string GetPercentualValue(CategoryWithAmountModel category)
    {
        var totalValueGetter = (CategoryWithAmountModel c) => c.Version == 1 ? c.TotalAmount.Value : c.Amount.Value;
        decimal percentage = 0;
        decimal total = Categories.Sum(c => totalValueGetter(c));
        if (total != 0)
            percentage = 100 / total * totalValueGetter(category);

        return percentage.ToString("0.##", CultureInfo.InvariantCulture);
    }

    public void Dispose()
        => UnBindEvents();

    private Price GetBalanceTotal()
    {
        var balance = IncomeTotal ?? Price.Zero(DefaultCurrency);
        balance -= ExistingExpenseTotal;

        if (ExpectedExpenseTotal != null)
            balance -= ExpectedExpenseTotal;
        
        return balance;
    }

    #region Navigations

    protected virtual string UrlSummary(T item) => null;

    protected virtual void OpenOverview(T item)
    { }

    protected virtual void OpenOverviewIncomes(T item)
    { }

    protected virtual void OpenOverview(T item, IKey categorykey)
    { }

    protected virtual void OpenNextPeriod() 
    { }

    protected virtual void OpenPrevPeriod() 
    { }

    #endregion

    #region Events

    private void BindEvents()
    {
        EventHandlers
            .Add<OutcomeCreated>(this)
            .Add<OutcomeDeleted>(this)
            .Add<OutcomeAmountChanged>(this)
            .Add<OutcomeWhenChanged>(this)
            .Add<PulledToRefresh>(this)
            .Add<SwipedLeft>(this)
            .Add<SwipedRight>(this)
            .Add<IncomeCreated>(this)
            .Add<IncomeDeleted>(this);
    }

    private void UnBindEvents()
    {
        EventHandlers
            .Remove<OutcomeCreated>(this)
            .Remove<OutcomeDeleted>(this)
            .Remove<OutcomeAmountChanged>(this)
            .Remove<OutcomeWhenChanged>(this)
            .Remove<PulledToRefresh>(this)
            .Remove<SwipedLeft>(this)
            .Remove<SwipedRight>(this)
            .Remove<IncomeCreated>(this)
            .Remove<IncomeDeleted>(this);
    }

    Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload)
    {
        OnMonthUpdatedEvent(payload.When);
        return Task.CompletedTask;
    }

    Task IEventHandler<OutcomeDeleted>.HandleAsync(OutcomeDeleted payload)
    {
        OnOutcomeDeletedEvent();
        return Task.CompletedTask;
    }

    Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload)
    {
        OnOutcomeAmountChangedEvent();
        return Task.CompletedTask;
    }

    Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload)
    {
        OnMonthUpdatedEvent(payload.When);
        return Task.CompletedTask;
    }

    async Task IEventHandler<PulledToRefresh>.HandleAsync(PulledToRefresh payload)
    {
        payload.IsHandled = true;
        await LoadSelectedPeriodAsync();
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

    async Task IEventHandler<IncomeCreated>.HandleAsync(IncomeCreated payload)
    {
        await LoadIncomeTotalAsync();
        StateHasChanged();
    }

    async Task IEventHandler<IncomeDeleted>.HandleAsync(IncomeDeleted payload)
    {
        await LoadIncomeTotalAsync();
        StateHasChanged();
    }

    private async void OnMonthUpdatedEvent(DateTime changed)
    {
        if (Periods != null && !IsContained(changed))
            IsPeriodReloadRequired = true;

        await LoadSelectedPeriodAsync();
        StateHasChanged();
    }

    protected virtual bool IsContained(DateTime changed)
        => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(IsContained)}'.");

    private async void OnOutcomeAmountChangedEvent()
    {
        await LoadSelectedPeriodAsync();
        StateHasChanged();
    }

    private async void OnOutcomeDeletedEvent()
    {
        IsPeriodReloadRequired = true;
        await LoadSelectedPeriodAsync();
        StateHasChanged();
    }

    #endregion
}
