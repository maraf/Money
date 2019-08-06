using Money.Models;
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

        public string UrlOverview(MonthModel month, IKey categoryKey)
        {
            if (categoryKey.IsEmpty)
                return UrlOverview(month);
            else
                return $"/{month.Year}/{month.Month}/overview/{categoryKey.AsGuidKey().Guid}";
        }

        public string UrlSearch()
            => $"/search";

        public string UrlCategories()
            => "/categories";

        public string UrlCurrencies()
            => "/currencies";

        public string UrlAbout()
            => "/about";

        public string UrlUserManage()
            => "/user";

        public string UrlUserPassword()
            => "/user/changepassword";

        public string UrlAccountLogin()
            => "/account/login";

        public string UrlAccountRegister()
            => "/account/register";

        #region External

        public string UrlMoneyProject()
            => "http://github.com/maraf/Money";

        public string UrlMoneyProjectIssueNew()
            => "https://github.com/maraf/Money/issues/new";

        #endregion
    }
}
