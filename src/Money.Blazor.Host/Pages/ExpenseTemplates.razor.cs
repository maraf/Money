using Money.Commands;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Models.Sorting;
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
using System.Linq;
using System.Threading.Tasks;

namespace Money.Pages;

public partial class ExpenseTemplates(
    ILog<ExpenseTemplates> Log,
    ICommandDispatcher Commands,
    IEventHandlerCollection EventHandlers,
    IQueryDispatcher Queries,
    CurrencyFormatterFactory CurrencyFormatterFactory,
    Navigator Navigator
) : 
    System.IDisposable, 
    IEventHandler<ExpenseTemplateCreated>,
    IEventHandler<ExpenseTemplateAmountChanged>,
    IEventHandler<ExpenseTemplateDescriptionChanged>,
    IEventHandler<ExpenseTemplateCategoryChanged>,
    IEventHandler<ExpenseTemplateRecurrenceChanged>,
    IEventHandler<ExpenseTemplateRecurrenceCleared>,
    IEventHandler<ExpenseTemplateDeleted>
{
    protected CurrencyFormatter CurrencyFormatter { get; private set; }
    protected ExpenseTemplateCreate CreateModal { get; set; }
    protected ExpenseTemplateDescription ChangeDescriptionModal { get; set; }
    protected ExpenseTemplateAmount ChangeAmountModal { get; set; }
    protected ExpenseTemplateCategory ChangeCategoryModal { get; set; }
    protected ExpenseTemplateRecurrence ChangeRecurrenceModal { get; set; }
    protected Confirm DeleteConfirm { get; set; }
    protected OutcomeCreate ExpenseModal { get; set; }
    protected List<ExpenseTemplateModel> AllModels { get; } = new List<ExpenseTemplateModel>();
    protected List<ExpenseTemplateModel> Models { get; } = new List<ExpenseTemplateModel>();
    protected SortDescriptor<ExpenseTemplateSortType> SortDescriptor { get; set; }
    protected LoadingContext Loading { get; } = new LoadingContext();

    protected string SearchQuery { get; set; }

    protected IKey ToDeleteKey { get; set; }
    protected ExpenseTemplateModel Selected { get; set; }
    protected string DeleteMessage { get; set; }

    protected async override Task OnInitializedAsync()
    {
        EventHandlers
            .Add<ExpenseTemplateCreated>(this)
            .Add<ExpenseTemplateAmountChanged>(this)
            .Add<ExpenseTemplateDescriptionChanged>(this)
            .Add<ExpenseTemplateCategoryChanged>(this)
            .Add<ExpenseTemplateRecurrenceChanged>(this)
            .Add<ExpenseTemplateRecurrenceCleared>(this)
            .Add<ExpenseTemplateDeleted>(this);

        await base.OnInitializedAsync();

        CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();
        SortDescriptor = await Queries.QueryAsync(new GetExpenseTemplateSortProperty());

        using (Loading.Start())
            await ReloadAsync();
    }

    public void Dispose() => EventHandlers
        .Remove<ExpenseTemplateCreated>(this)
        .Remove<ExpenseTemplateAmountChanged>(this)
        .Remove<ExpenseTemplateDescriptionChanged>(this)
        .Remove<ExpenseTemplateCategoryChanged>(this)
        .Remove<ExpenseTemplateRecurrenceChanged>(this)
        .Remove<ExpenseTemplateRecurrenceCleared>(this)
        .Remove<ExpenseTemplateDeleted>(this);

    protected string DayInMonth(int day) => day switch
    {
        1 => "1st",
        2 => "2nd",
        3 => "3rd",
        _ => $"{day}th"
    };

    protected string MonthInYear(int month) => month switch
    {
        1 => "January",
        2 => "February",
        3 => "March",
        4 => "April",
        5 => "May",
        6 => "June",
        7 => "July",
        8 => "August",
        9 => "September",
        10 => "October",
        11 => "November",
        12 => "December",
        _ => "---"
    };

    private async Task ReloadAsync()
    {
        AllModels.Clear();
        AllModels.AddRange(await Queries.QueryAsync(ListAllExpenseTemplate.Version4(SortDescriptor)));
        OnSearch();
        StateHasChanged();
    }

    protected void OnSortChanged() 
        => _ = ReloadAsync();

    protected void Delete()
    {
        _ = Commands.HandleAsync(new DeleteExpenseTemplate(ToDeleteKey));
    }

    protected void Edit(ExpenseTemplateModel selected, ModalDialog modal)
    {
        Selected = selected; 
        modal.Show(); 
        StateHasChanged();
    }

    protected Task OnEventAsync()
    {
        Log.Debug($"OnEventAsync");
        _ = ReloadAsync();
        return Task.CompletedTask;
    }

    protected void OnSearch()
    {
        Log.Debug($"OnSearch '{SearchQuery}'");

        Models.Clear();
        if (String.IsNullOrEmpty(SearchQuery))
        {
            Models.AddRange(AllModels);
            return;
        }

        string searchQuery = SearchQuery.ToLower().Trim();
        Models.AddRange(AllModels.Where(m => m.Description.Contains(SearchQuery, StringComparison.CurrentCultureIgnoreCase)));
    }

    public Task HandleAsync(ExpenseTemplateCreated payload) => OnEventAsync();
    public Task HandleAsync(ExpenseTemplateDescriptionChanged payload) => OnEventAsync();
    public Task HandleAsync(ExpenseTemplateDeleted payload) => OnEventAsync();
    public Task HandleAsync(ExpenseTemplateAmountChanged payload) => OnEventAsync();
    public Task HandleAsync(ExpenseTemplateCategoryChanged payload) => OnEventAsync();
    public Task HandleAsync(ExpenseTemplateRecurrenceChanged payload) => OnEventAsync();
    public Task HandleAsync(ExpenseTemplateRecurrenceCleared payload) => OnEventAsync();
}