using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Queries;
using Money.Services;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Money.Components;

public partial class ExpenseCreateSwitcher(
    ILog<ExpenseCreateSwitcher> Log,
    IQueryDispatcher Queries,
    IEventHandlerCollection EventHandlers,
    Navigator Navigator,
    NavigationManager NavigationManager
) : IEventHandler<UserPropertyChanged>, System.IDisposable
{
    protected ExpenseCreateDialogType Selected { get; set; }

    [CascadingParameter]
    public Navigator.ComponentContainer ComponentContainer { get; set; }

    private bool isInitialized;

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ReloadAsync();

        EventHandlers.Add<UserPropertyChanged>(this);
        Navigator.LocationChanged += OnLocationChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!isInitialized)
        {
            isInitialized = true;
            await TryOpenFromUrlAsync();
        }
    }

    public void Dispose()
    {
        Navigator.LocationChanged -= OnLocationChanged;
        EventHandlers.Remove<UserPropertyChanged>(this);
    }

    private async Task ReloadAsync()
    {
        Selected = await Queries.QueryAsync(new GetExpenseCreateDialogTypeProperty());
        Log.Debug($"User prefered ExpenseCreateDialog is '{Selected}'");
    }

    async Task IEventHandler<UserPropertyChanged>.HandleAsync(UserPropertyChanged payload)
    {
        Log.Debug($"Got UserPropertyChanged event for propertyKey '{payload.PropertyKey}' with value '{payload.Value}'");
        if (payload.PropertyKey == GetExpenseCreateDialogTypeProperty.PropertyKey)
        {
            await ReloadAsync();
            StateHasChanged();
        }
    }

    private async void OnLocationChanged(string url)
    {
        await TryOpenFromUrlAsync();
    }

    private async Task TryOpenFromUrlAsync()
    {
        var amountStr = Navigator.FindQueryParameter("expense-amount");
        if (string.IsNullOrEmpty(amountStr))
            return;

        Log.Debug($"Found expense-amount query parameter: '{amountStr}'");

        if (!decimal.TryParse(amountStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var amountValue))
        {
            Log.Debug($"Failed to parse expense-amount: '{amountStr}'");
            return;
        }

        var description = Navigator.FindQueryParameter("expense-description") ?? string.Empty;
        var categoryParam = Navigator.FindQueryParameter("expense-category");
        var currencyParam = Navigator.FindQueryParameter("expense-currency");
        var whenParam = Navigator.FindQueryParameter("expense-when");
        var fixedParam = Navigator.FindQueryParameter("expense-fixed");

        // Resolve currency
        string currency = null;
        if (!string.IsNullOrEmpty(currencyParam))
        {
            currency = currencyParam;
        }
        else
        {
            var currencies = await Queries.QueryAsync(new ListAllCurrency());
            currency = currencies?.FirstOrDefault()?.UniqueCode;
            if (currency == null)
            {
                Log.Debug("No currencies available, cannot open expense create from URL");
                return;
            }
        }

        var amount = new Price(amountValue, currency);

        // Resolve category
        IKey categoryKey = KeyFactory.Empty(typeof(Category));
        if (!string.IsNullOrEmpty(categoryParam))
        {
            if (Guid.TryParse(categoryParam, out var categoryGuid))
            {
                categoryKey = GuidKey.Create(categoryGuid, KeyFactory.Empty(typeof(Category)).Type);
            }
            else
            {
                var categories = await Queries.QueryAsync(new ListAllCategory());
                var match = categories?.FirstOrDefault(c => string.Equals(c.Name, categoryParam, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                    categoryKey = match.Key;
                else
                    Log.Debug($"Category not found by name: '{categoryParam}'");
            }
        }

        // Parse when
        DateTime when = AppDateTime.Today;
        if (!string.IsNullOrEmpty(whenParam))
        {
            if (DateTime.TryParseExact(whenParam, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedWhen))
                when = parsedWhen;
            else
                Log.Debug($"Failed to parse expense-when: '{whenParam}'");
        }

        // Parse fixed
        bool isFixed = string.Equals(fixedParam, "true", StringComparison.OrdinalIgnoreCase);

        // Remove query parameters from URL
        RemoveExpenseQueryParameters();

        // Open the dialog
        Log.Debug($"Opening expense create from URL: amount={amount}, description='{description}', category={categoryKey}, when={when}, fixed={isFixed}");
        ComponentContainer?.ExpenseCreate?.Show(amount, description, categoryKey, when, isFixed);
    }

    private void RemoveExpenseQueryParameters()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        var expenseKeys = queryParams.Keys.Where(k => k.StartsWith("expense-", StringComparison.OrdinalIgnoreCase)).ToList();
        foreach (var key in expenseKeys)
            queryParams.Remove(key);

        string newUrl;
        if (queryParams.Count == 0)
        {
            newUrl = uri.GetLeftPart(UriPartial.Path);
        }
        else
        {
            newUrl = uri.GetLeftPart(UriPartial.Path);
            foreach (var kvp in queryParams)
            {
                foreach (var value in kvp.Value)
                    newUrl = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(newUrl, kvp.Key, value);
            }
        }

        // Make relative for NavigateTo
        var baseUri = NavigationManager.BaseUri;
        if (newUrl.StartsWith(baseUri))
            newUrl = newUrl.Substring(baseUri.Length - 1); // keep leading /

        NavigationManager.NavigateTo(newUrl, new NavigationOptions { ReplaceHistoryEntry = true });
    }
}