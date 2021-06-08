using Microsoft.AspNetCore.WebUtilities;
using Money.Models;
using Money.Models.Sorting;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class NavigatorUrl
    {
        public string UrlSummary()
            => "/";

        public string UrlSummary(YearModel year)
            => $"/{year.Year}";

        public string UrlSummary(MonthModel month)
            => $"/{month.Year}/{month.Month}";

        public string UrlOverview(MonthModel month)
            => $"/{month.Year}/{month.Month}/overview";

        public string UrlOverviewIncomes(MonthModel month)
            => $"/{month.Year}/{month.Month}/overview/incomes";

        public string UrlOverview(YearModel year)
            => $"/{year.Year}/overview";

        public string UrlOverview(MonthModel month, IKey categoryKey)
        {
            if (categoryKey.IsEmpty)
                return UrlOverview(month);
            else
                return $"/{month.Year}/{month.Month}/overview/{categoryKey.AsGuidKey().Guid}";
        }

        public string UrlOverview(YearModel year, IKey categoryKey)
        {
            if (categoryKey.IsEmpty)
                return UrlOverview(year);
            else
                return $"/{year.Year}/overview/{categoryKey.AsGuidKey().Guid}";
        }

        public string UrlTrends()
            => $"/trends";

        public string UrlTrends(IKey categoryKey) 
            => $"/trends/{categoryKey.AsGuidKey().Guid}";

        public string UrlTrends(YearModel year, IKey categoryKey) 
            => $"/{year.Year}/trends/{categoryKey.AsGuidKey().Guid}";

        public string UrlSearch(string query = null, SortDescriptor<OutcomeOverviewSortType> sortDescriptor = null)
        {
            string url = "/search";
            if (!String.IsNullOrEmpty(query))
                url = QueryHelpers.AddQueryString(url, "q", query);

            if (sortDescriptor != null)
                url = QueryHelpers.AddQueryString(url, "sort", sortDescriptor.ToUrlString());

            return url;
        }

        public string UrlCategories()
            => "/categories";

        public string UrlCurrencies()
            => "/currencies";

        public string UrlAbout()
            => "/about";

        public string UrlUserProfile()
            => "/user";

        public string UrlUserPassword()
            => "/user/changepassword";

        public string UrlUserSettings()
            => "/user/settings";

        public string UrlAccountLogin()
            => "/account/login";

        public string UrlAccountRegister()
            => "/account/register";

        #region External

        public string UrlMoneyProject()
            => "http://money.neptuo.com";
        
        public string UrlGithubRepository()
            => "http://github.com/maraf/Money";

        public string UrlGithubRepositoryIssueNew(string title = null)
            => AddOptionalParameter("https://github.com/maraf/Money/issues/new", "title", title);

        public string AddOptionalParameter(string url, string name, string value)
        {
            if (String.IsNullOrEmpty(value))
                return url;

            value = Uri.EscapeUriString(value);

            char separator = url.Contains('?') ? '&' : '?';
            return $"{url}{separator}{name}={value}";
        }

        #endregion
    }
}
