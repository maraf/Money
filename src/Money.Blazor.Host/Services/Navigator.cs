﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
using Money.Components;
using Money.Layouts;
using Money.Models;
using Money.Models.Sorting;
using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class Navigator : NavigatorUrl, System.IDisposable
    {
        private readonly NavigationManager manager;
        private readonly ComponentContainer componentContainer;
        private readonly Interop interop;
        private readonly IJSRuntime js;
        private Dictionary<string, StringValues> queryString;

        public event Action<string> LocationChanged;

        public Navigator(NavigationManager manager, ComponentContainer componentContainer, Interop interop, IJSRuntime js)
        {
            Ensure.NotNull(manager, "manager");
            Ensure.NotNull(interop, "interop");
            Ensure.NotNull(componentContainer, "modalContainer");
            Ensure.NotNull(js, "js");
            this.manager = manager;
            this.componentContainer = componentContainer;
            this.interop = interop;
            this.js = js;

            manager.LocationChanged += OnLocationChanged;
        }

        public void Dispose()
        {
            manager.LocationChanged -= OnLocationChanged;
        }

        public string UrlCurrent()
            => manager.Uri;

        public string UrlOrigin()
            => manager.BaseUri;

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            queryString = null;
            LocationChanged?.Invoke(e.Location);
        }

        public async Task ReloadAsync()
            => await js.InvokeVoidAsync("window.location.reload");

        public async Task AlertAsync(string message)
            => await js.InvokeVoidAsync("window.alert", message);

        private void OpenExternal(string url)
            => interop.NavigateTo(url);

        public void OpenExpenseCreate()
            => componentContainer.ExpenseCreate?.Show();

        public void OpenExpenseCreate(IKey categoryKey)
            => componentContainer.ExpenseCreate?.Show(categoryKey);

        public void OpenExpenseCreate(Price amount, string description, IKey categoryKey, bool isFixed) 
            => componentContainer.ExpenseCreate?.Show(amount, description, categoryKey, isFixed);

        public void OpenExpenseCreate(Price amount, string description, IKey categoryKey, DateTime when, bool isFixed) 
            => componentContainer.ExpenseCreate?.Show(amount, description, categoryKey, when, isFixed);

        public void Open(string url)
            => manager.NavigateTo(url);

        public void OpenSummary()
            => manager.NavigateTo(UrlSummary());

        public void OpenSummary(MonthModel month)
            => manager.NavigateTo(UrlSummary(month));

        public void OpenSummary(YearModel year)
            => manager.NavigateTo(UrlSummary(year));

        public void OpenOverview(MonthModel month)
            => manager.NavigateTo(UrlOverview(month));

        public void OpenOverviewIncomes(MonthModel month)
            => manager.NavigateTo(UrlOverviewIncomes(month));

        public void OpenChecklist(MonthModel month)
            => manager.NavigateTo(UrlChecklist(month));

        public void OpenOverview(MonthModel month, IKey categoryKey)
            => manager.NavigateTo(UrlOverview(month, categoryKey));

        public void OpenOverview(YearModel Year)
            => manager.NavigateTo(UrlOverview(Year));

        public void OpenOverview(YearModel Year, IKey categoryKey)
            => manager.NavigateTo(UrlOverview(Year, categoryKey));

        public void OpenTrends()
            => manager.NavigateTo(UrlTrends());

        public void OpenTrends(IKey categoryKey)
            => manager.NavigateTo(UrlTrends(categoryKey));

        public void OpenTrends(YearModel year, IKey categoryKey)
            => manager.NavigateTo(UrlTrends(year, categoryKey));

        public void OpenBalances(YearModel year)
            => manager.NavigateTo(UrlBalances(year));

        public void OpenSearch(string searchText = null, SortDescriptor<OutcomeOverviewSortType> sortDescriptor = null)
            => manager.NavigateTo(UrlSearch(searchText, sortDescriptor));

        public void OpenCategories()
            => manager.NavigateTo(UrlCategories());

        public void OpenCurrencies()
            => manager.NavigateTo(UrlCurrencies());

        public void OpenExpenseTemplates()
            => manager.NavigateTo(UrlExpenseTemplates());

        public void OpenExpenseTemplateCalendar(IKey expenseTemplateKey)
            => manager.NavigateTo(UrlExpenseTemplateCalendar(expenseTemplateKey));

        public void OpenExpenseTemplateCalendar(IKey expenseTemplateKey, YearModel year)
            => manager.NavigateTo(UrlExpenseTemplateCalendar(expenseTemplateKey, year));

        public void OpenAbout()
            => manager.NavigateTo(UrlAbout());

        public void OpenUserManage()
            => manager.NavigateTo(UrlUserProfile());

        public void OpenUserPassword()
            => manager.NavigateTo(UrlUserPassword());

        public void OpenUserSettings()
            => manager.NavigateTo(UrlUserSettings());

        public void OpenLogin()
        {
            string loginUrl = UrlAccountLogin();
            string currentUrl = GetCurrentUrl();

            if (loginUrl != currentUrl && currentUrl != UrlAccountRegister())
                manager.NavigateTo($"{loginUrl}?returnUrl={currentUrl}");
        }

        public bool IsLoginUrl()
            => GetCurrentUrl() == UrlAccountLogin();

        private string GetCurrentUrl()
        {
            string currentUrl = "/" + manager.ToBaseRelativePath(manager.BaseUri);
            int indexOfReturnUrl = currentUrl.IndexOf("?returnUrl");
            if (indexOfReturnUrl >= 0)
                currentUrl = currentUrl.Substring(0, indexOfReturnUrl);

            return currentUrl;
        }

        public void OpenRegister()
            => manager.NavigateTo(UrlAccountRegister());

        public Dictionary<string, StringValues> GetQueryString()
        {
            if (queryString == null)
            {
                var absolute = manager.ToAbsoluteUri(manager.Uri);
                queryString = QueryHelpers.ParseQuery(absolute.Query);
            }

            return queryString;
        }

        public string FindQueryParameter(string name)
        {
            if (GetQueryString().TryGetValue(name, out var value))
                return value;

            return null;
        }

        public void ToggleMainMenu()
        {
            if (componentContainer.MainMenu != null)
                componentContainer.MainMenu.UpdateMainMenuVisible(!componentContainer.MainMenu.IsMainMenuVisible);
        }

        public class ComponentContainer
        {
            public IExpenseCreateNavigator ExpenseCreate { get; set; }
            public IMainMenu MainMenu { get; internal set; }
        }
    }
}
