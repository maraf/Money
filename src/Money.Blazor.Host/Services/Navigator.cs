using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Money.Models;
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
        private readonly Interop interop;
        private readonly IJSRuntime jsRuntime;

        public event Action<string> LocationChanged;

        public Navigator(NavigationManager manager, Interop interop, IJSRuntime jsRuntime)
        {
            Ensure.NotNull(manager, "manager");
            Ensure.NotNull(interop, "interop");
            Ensure.NotNull(jsRuntime, "jsRuntime");
            this.manager = manager;
            this.interop = interop;
            this.jsRuntime = jsRuntime;

            manager.LocationChanged += OnLocationChanged;
        }

        public void Dispose()
        {
            manager.LocationChanged -= OnLocationChanged;
        }

        public string UrlCurrent() 
            => manager.Uri;

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
            => LocationChanged?.Invoke(e.Location);

        public async Task ReloadAsync()
            => await jsRuntime.InvokeVoidAsync("window.location.reload");

        public async Task AlertAsync(string message)
            => await jsRuntime.InvokeVoidAsync("window.alert", message);

        private void OpenExternal(string url)
            => interop.NavigateTo(url);

        public void Open(string url)
            => manager.NavigateTo(url);

        public void OpenSummary()
            => manager.NavigateTo(UrlSummary());

        public void OpenSummary(MonthModel month)
            => manager.NavigateTo(UrlSummary(month));

        public void OpenOverview(MonthModel month)
            => manager.NavigateTo(UrlOverview(month));

        public void OpenOverviewIncomes(MonthModel month)
            => manager.NavigateTo(UrlOverviewIncomes(month));

        public void OpenOverview(MonthModel month, IKey categoryKey)
            => manager.NavigateTo(UrlOverview(month, categoryKey));

        public void OpenOverview(YearModel Year)
            => manager.NavigateTo(UrlOverview(Year));

        public void OpenOverview(YearModel Year, IKey categoryKey)
            => manager.NavigateTo(UrlOverview(Year, categoryKey));

        public void OpenTrends(YearModel Year, IKey categoryKey)
            => manager.NavigateTo(UrlTrends(Year, categoryKey));

        public void OpenTrends(IKey categoryKey)
            => manager.NavigateTo(UrlTrends(categoryKey));

        public void OpenSearch()
            => manager.NavigateTo(UrlSearch());

        public void OpenCategories()
            => manager.NavigateTo(UrlCategories());

        public void OpenCurrencies()
            => manager.NavigateTo(UrlCurrencies());

        public void OpenAbout()
            => manager.NavigateTo(UrlAbout());

        public void OpenUserManage()
            => manager.NavigateTo(UrlUserManage());

        public void OpenUserPassword()
            => manager.NavigateTo(UrlUserPassword());

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
    }
}
