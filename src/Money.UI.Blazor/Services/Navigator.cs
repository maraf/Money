using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
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
        private readonly Interop interop;

        public event Action<string> LocationChanged;

        public Navigator(IUriHelper uri, Interop interop)
        {
            Ensure.NotNull(uri, "uri");
            Ensure.NotNull(interop, "interop");
            this.uri = uri;
            this.interop = interop;

            uri.OnLocationChanged += OnLocationChanged;
        }

        public void Dispose()
        {
            uri.OnLocationChanged -= OnLocationChanged;
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
            => LocationChanged?.Invoke(e.Location);

        private void OpenExternal(string url)
            => interop.NavigateTo(url);

        public void Open(string url)
            => uri.NavigateTo(url);

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
        {
            string loginUrl = UrlAccountLogin();
            string currentUrl = GetCurrentUrl();

            if (loginUrl != currentUrl && currentUrl != UrlAccountRegister())
                uri.NavigateTo($"{loginUrl}?returnUrl={currentUrl}");
        }

        private string GetCurrentUrl()
        {
            string currentUrl = "/" + uri.ToBaseRelativePath(uri.GetBaseUri(), uri.GetAbsoluteUri());
            int indexOfReturnUrl = currentUrl.IndexOf("?returnUrl");
            if (indexOfReturnUrl >= 0)
                currentUrl = currentUrl.Substring(0, indexOfReturnUrl);

            return currentUrl;
        }

        public void OpenRegister()
            => uri.NavigateTo(UrlAccountRegister());
    }
}
