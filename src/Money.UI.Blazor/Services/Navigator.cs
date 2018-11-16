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
    internal class Navigator
    {
        private readonly IUriHelper uri;

        public Navigator(IUriHelper uri)
        {
            Ensure.NotNull(uri, "uri");
            this.uri = uri;
        }

        private void OpenExternal(string url)
            => JSRuntime.Current.InvokeAsync<bool>("Money.NavigateTo", url);

        public void OpenSummary()
            => uri.NavigateTo(UrlSummary());

        public string UrlSummary()
            => "/";

        public void OpenSummary(MonthModel month)
            => uri.NavigateTo(UrlSummary(month));

        public string UrlSummary(MonthModel month)
            => $"/{month.Year}/{month.Month}";

        public void OpenOverview(MonthModel month)
            => uri.NavigateTo(UrlOverview(month));

        public string UrlOverview(MonthModel month)
            => $"/{month.Year}/{month.Month}/overview";

        public void OpenOverview(MonthModel month, IKey categoryKey)
            => uri.NavigateTo(UrlOverview(month, categoryKey));

        public string UrlOverview(MonthModel month, IKey categoryKey)
        {
            if (categoryKey.IsEmpty)
                return UrlOverview(month);
            else
                return $"/{month.Year}/{month.Month}/overview/{categoryKey.AsGuidKey().Guid}";
        }


        public void OpenCategories()
            => uri.NavigateTo(UrlCategories());

        public string UrlCategories()
            => "/categories";


        public void OpenCurrencies()
            => uri.NavigateTo(UrlCurrencies());

        public string UrlCurrencies()
            => "/currencies";


        public void OpenAbout()
            => uri.NavigateTo(UrlAbout());

        public string UrlAbout()
            => "/about";


        public void OpenUserManage()
            => OpenExternal(UrlUserManage());

        public string UrlUserManage()
            => "/manage";

        #region External

        public string UrlMoneyProject()
            => "http://github.com/maraf/Money";

        public string UrlMoneyProjectIssueNew()
            => "https://github.com/maraf/Money/issues/new";

        #endregion
    }
}
