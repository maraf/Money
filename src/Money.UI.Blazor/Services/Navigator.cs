using Microsoft.AspNetCore.Blazor.Services;
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
    internal class Navigator : NavigatorUrl, System.IDisposable
    {
        private readonly IUriHelper uri;

        public event Action<string> LocationChanged;

        public Navigator(IUriHelper uri)
        {
            Ensure.NotNull(uri, "uri");
            this.uri = uri;

            uri.OnLocationChanged += OnLocationChanged;
        }

        public void Dispose()
        {
            uri.OnLocationChanged -= OnLocationChanged;
        }

        private void OnLocationChanged(object sender, string e)
            => LocationChanged?.Invoke(e);

        private void OpenExternal(string url)
            => JSRuntime.Current.InvokeAsync<bool>("Money.NavigateTo", url);

        public void OpenSummary()
            => uri.NavigateTo(UrlSummary());

        public void OpenSummary(MonthModel month)
            => uri.NavigateTo(UrlSummary(month));

        public void OpenOverview(MonthModel month)
            => uri.NavigateTo(UrlOverview(month));

        public void OpenOverview(MonthModel month, IKey categoryKey)
            => uri.NavigateTo(UrlOverview(month, categoryKey));

        public void OpenSearch()
            => uri.NavigateTo(UrlSearch());

        public void OpenCategories()
            => uri.NavigateTo(UrlCategories());

        public void OpenCurrencies()
            => uri.NavigateTo(UrlCurrencies());

        public void OpenAbout()
            => uri.NavigateTo(UrlAbout());

        public void OpenUserManage()
            => uri.NavigateTo(UrlUserManage());

        public void OpenUserPassword()
            => uri.NavigateTo(UrlUserPassword());

        public void OpenLogin()
            => uri.NavigateTo("/account/login");
    }
}
