using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Components.Bootstrap;
using Money.Components.Settings;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Queries;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages.Users;

public partial class Settings(
    ILog<Settings> Log,
    IQueryDispatcher Queries,
    ICommandDispatcher Commands,
    IEventHandlerCollection EventHandlers
) : 
    System.IDisposable,
    IEventHandler<UserPropertyChanged>
{
    protected PropertyViewModel PriceDecimals { get; set; }
    protected PropertyDialog PriceDecimalsEditor { get; set; }

    protected PropertyViewModel DateFormat { get; set; }
    protected PropertyDialog DateFormatEditor { get; set; }

    protected MobileMenuPropertyViewModel MobileMenu { get; set; }
    protected PropertyDialog MobileMenuEditor { get; set; }

    protected SortPropertyViewModel<SummarySortType> SummarySort { get; set; }
    protected PropertyDialog SummarySortEditor { get; set; }

    protected SortPropertyViewModel<OutcomeOverviewSortType> ExpenseOverviewSort { get; set; }
    protected PropertyDialog ExpenseOverviewSortEditor { get; set; }

    protected SortPropertyViewModel<OutcomeOverviewSortType> SearchSort { get; set; }
    protected PropertyDialog SearchSortEditor { get; set; }

    protected EnumPropertyViewModel<BalanceDisplayType> BalanceDisplay { get; set; }
    protected PropertyDialog BalanceDisplayEditor { get; set; }

    protected EnumPropertyViewModel<SummaryDisplayType> SummaryDisplay { get; set; }
    protected PropertyDialog SummaryDisplayEditor { get; set; }

    protected SortPropertyViewModel<ExpenseTemplateSortType> ExpenseTemplateSort { get; set; }
    protected PropertyDialog ExpenseTemplateSortEditor { get; set; }

    protected EnumPropertyViewModel<ExpenseCreateDialogType> ExpenseDialogCreate { get; set; }
    protected PropertyDialog ExpenseDialogCreateEditor { get; set; }

    protected EnumPropertyViewModel<ThemeType> Theme { get; set; }
    protected PropertyDialog ThemeEditor { get; set; }

    protected List<UserPropertyModel> Models { get; set; }
    protected List<PropertyViewModel> ViewModels { get; } = new List<PropertyViewModel>();

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        EventHandlers.Add<UserPropertyChanged>(this);

        PriceDecimals = AddProperty("PriceDecimalDigits", "Price decimal digits", () => PriceDecimalsEditor.Show(), icon: "pound-sign", defaultValue: "2");
        DateFormat = AddProperty("DateFormat", "Date format", () => DateFormatEditor.Show(), icon: "calendar-day", defaultValue: CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern);
        MobileMenu = AddProperty<MobileMenuPropertyViewModel>("MobileMenu", "Mobile menu", () => MobileMenuEditor.Show(), icon: "mobile");
        SummarySort = AddProperty<SortPropertyViewModel<SummarySortType>>("SummarySort", "Summary sort", () => SummarySortEditor.Show(), icon: "sort-alpha-down", defaultValue: "ByCategory-Ascending");
        SummaryDisplay = AddProperty<EnumPropertyViewModel<SummaryDisplayType>>("SummaryDisplay", "Summary display", () => SummaryDisplayEditor.Show(), icon: "eye", defaultValue: "Total");
        ExpenseOverviewSort = AddProperty<SortPropertyViewModel<OutcomeOverviewSortType>>("ExpenseOverviewSort", "Expense overview sort", () => ExpenseOverviewSortEditor.Show(), icon: "sort-alpha-down", defaultValue: "ByWhen-Descending");
        SearchSort = AddProperty<SortPropertyViewModel<OutcomeOverviewSortType>>("SearchSort", "Search sort", () => SearchSortEditor.Show(), icon: "sort-alpha-down", defaultValue: "ByWhen-Descending");
        BalanceDisplay = AddProperty<EnumPropertyViewModel<BalanceDisplayType>>("BalanceDisplay", "Balance display", () => BalanceDisplayEditor.Show(), icon: "eye", defaultValue: "Total");
        ExpenseTemplateSort = AddProperty<SortPropertyViewModel<ExpenseTemplateSortType>>("ExpenseTemplateSort", "ExpenseTemplate sort", () => ExpenseTemplateSortEditor.Show(), icon: "sort-alpha-down", defaultValue: "ByDescription-Ascending");
        ExpenseDialogCreate = AddProperty<EnumPropertyViewModel<ExpenseCreateDialogType>>("ExpenseCreateDialog", "Expense create dialog type", () => ExpenseDialogCreateEditor.Show(), icon: "minus-circle", defaultValue: "Standard");
        Theme = AddProperty<EnumPropertyViewModel<ThemeType>>(GetThemeTypeProperty.PropertyKey, "Color theme", () => ThemeEditor.Show(), icon: "palette", defaultValue: "Light");

        await LoadAsync();
    }

    public void Dispose()
    {
        EventHandlers.Remove<UserPropertyChanged>(this);
    }

    private PropertyViewModel AddProperty(string propertyKey, string title, Action edit, string defaultValue = null, string icon = null)
        => AddProperty<PropertyViewModel>(propertyKey, title, edit, defaultValue, icon);

    private T AddProperty<T>(string propertyKey, string title, Action edit, string defaultValue = null, string icon = null)
        where T : PropertyViewModel, new()
    {
        var viewModel = ViewModels.FirstOrDefault(vm => vm.Key == propertyKey);
        if (viewModel == null)
        {
            ViewModels.Add(viewModel = new T()
            {
                Commands = Commands,
                Queries = Queries
            });
        }

        viewModel.Key = propertyKey;
        viewModel.Title = title;
        viewModel.Icon = icon;
        viewModel.Edit = edit;
        viewModel.DefaultValue = defaultValue;

        return (T)viewModel;
    }

    Task IEventHandler<UserPropertyChanged>.HandleAsync(UserPropertyChanged payload)
    {
        var viewModel = ViewModels.FirstOrDefault(vm => vm.Key == payload.PropertyKey);
        if (viewModel != null)
        {
            Log.Debug($"Changing property '{payload.PropertyKey}' value to '{payload.Value}'");

            viewModel.Model.Value = payload.Value;
            viewModel.CurrentValue = null;
        }

        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task LoadAsync()
    {
        Models = await Queries.QueryAsync(new ListUserProperty());
        foreach (var model in Models)
        {
            var viewModel = ViewModels.FirstOrDefault(vm => vm.Key == model.Key);
            if (viewModel != null)
                viewModel.Model = model;
        }

        foreach (var viewModel in ViewModels)
            await viewModel.InitializeAsync();
    }
}

public class PropertyViewModel
{
    public ICommandDispatcher Commands { get; set; }
    public IQueryDispatcher Queries { get; set; }

    public string Key { get; set; }
    public string Title { get; set; }
    public string Icon { get; set; }
    public Action Edit { get; set; }
    public UserPropertyModel Model { get; set; }
    public string DefaultValue { get; set; }

    private string currentValue = null;

    public string CurrentValue
    {
        get => currentValue ?? Model?.Value ?? DefaultValue;
        set => currentValue = value;
    }

    public virtual async Task SetAsync()
    {
        Console.WriteLine($"Current '{currentValue}', ModelValue '{Model?.Value}'.");

        if (String.IsNullOrEmpty(currentValue) || currentValue == DefaultValue)
            currentValue = null;

        if (Model == null || currentValue != Model.Value)
        {
            Console.WriteLine("Send command.");
            await Commands.HandleAsync(new SetUserProperty(Key, currentValue));
        }
    }

    public virtual Task InitializeAsync()
        => Task.CompletedTask;
}

public class MobileMenuPropertyViewModel : PropertyViewModel
{
    public List<IAvailableMenuItemModel> AvailableModels { get; set; }
    public List<string> SelectedIdentifiers { get; set; }

    public async override Task InitializeAsync()
    {
        await base.InitializeAsync();

        AvailableModels = await Queries.QueryAsync(new ListAvailableMenuItem());

        SelectedIdentifiers = CurrentValue != null
            ? CurrentValue.Split(',').ToList()
            : new List<string>(0);
    }

    public override Task SetAsync()
    {
        CurrentValue = String.Join(",", AvailableModels.Where(m => SelectedIdentifiers.Contains(m.Identifier)).Select(m => m.Identifier));
        return base.SetAsync();
    }
}

public class SortPropertyViewModel<T> : PropertyViewModel
    where T : struct
{
    public T Property { get; set; }
    public SortDirection Direction { get; set; }

    public async override Task InitializeAsync()
    {
        await base.InitializeAsync();

        if (CurrentValue != null)
        {
            string[] parts = CurrentValue.Split('-');
            Property = Enum.Parse<T>(parts[0]);
            Direction = Enum.Parse<SortDirection>(parts[1]);
        }
    }

    public override Task SetAsync()
    {
        CurrentValue = $"{Property}-{Direction}";
        return base.SetAsync();
    }
}

public class EnumPropertyViewModel<T> : PropertyViewModel
    where T : struct
{
    public T Property { get; set; }

    public async override Task InitializeAsync()
    {
        await base.InitializeAsync();

        if (CurrentValue != null)
            Property = Enum.Parse<T>(CurrentValue);
    }

    public override Task SetAsync()
    {
        CurrentValue = $"{Property}";
        return base.SetAsync();
    }
}
