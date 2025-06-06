using Microsoft.AspNetCore.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Queries;
using Money.Services;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Money.Pages;

public partial class ExpenseTemplateCalendar(IQueryDispatcher Queries, Navigator Navigator, IEventHandlerCollection EventHandlers) : 
    System.IDisposable,
    IEventHandler<SwipedLeft>,
    IEventHandler<SwipedRight>
{
    [Parameter]
    public Guid ExpenseTemplateGuid { get; set; }

    [Parameter]
    public int Year { get; set; }

    protected IKey ExpenseTemplateKey { get; set; }
    protected YearModel SelectedPeriod { get; set; }
    protected ExpenseTemplateCalendarDisplayType SelectedDisplayType { get; set; }
    protected List<YearModel> PeriodGuesses { get; set; }
    protected List<ExpenseTemplateCalendarMonthModel> Models { get; set; }
    protected LoadingContext Loading { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        EventHandlers
            .Add<SwipedLeft>(this)
            .Add<SwipedRight>(this);

        await base.OnInitializedAsync();

        SelectedDisplayType = await Queries.QueryAsync(new GetExpenseTemplateCalendarDisplayProperty());
    }

    public void Dispose()
    {
        EventHandlers
            .Remove<SwipedLeft>(this)
            .Remove<SwipedRight>(this);
    }

    protected async override Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        ExpenseTemplateKey = GuidKey.Create(ExpenseTemplateGuid, KeyFactory.Empty(typeof(Category)).Type);
        SelectedPeriod = new YearModel(Year);
        PeriodGuesses = new List<YearModel>(2)
        {
            SelectedPeriod - 1,
            SelectedPeriod - 2,
        };

        await LoadAsync();
    }

    protected async Task LoadAsync()
    {
        using (Loading.Start())
        {
            Models = null;
            Models = await Queries.QueryAsync(new ListYearExpenseTemplateCalendar(SelectedPeriod, ExpenseTemplateKey));
        }
    }

    protected Task<IReadOnlyCollection<YearModel>> GetYearsAsync() => Task.FromResult<IReadOnlyCollection<YearModel>>([
        AppDateTime.Today.AddYears(1),
        AppDateTime.Today,
        AppDateTime.Today.AddYears(-1),
        AppDateTime.Today.AddYears(-2),
        AppDateTime.Today.AddYears(-3),
        AppDateTime.Today.AddYears(-4),
        AppDateTime.Today.AddYears(-5),
        AppDateTime.Today.AddYears(-6),
        AppDateTime.Today.AddYears(-7),
    ]);

    Task IEventHandler<SwipedLeft>.HandleAsync(SwipedLeft payload)
    {
        Navigator.OpenExpenseTemplateCalendar(ExpenseTemplateKey, SelectedPeriod - 1);
        return Task.CompletedTask;
    }

    Task IEventHandler<SwipedRight>.HandleAsync(SwipedRight payload)
    {
        Navigator.OpenExpenseTemplateCalendar(ExpenseTemplateKey, SelectedPeriod + 1);
        return Task.CompletedTask;
    }
}