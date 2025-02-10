using Money.Models;
using Money.Models.Queries;
using Money.Pages;
using Money.Services;
using Neptuo;
using Neptuo.Queries;
using Neptuo.Queries.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    internal class MenuItemService : HttpQueryDispatcher.IMiddleware
    {
        private const string DefaultValue = "main-menu,summary-month,expense-create";

        protected static readonly YearModel ThisYear = new YearModel(DateTime.Today.Year);

        private readonly List<MenuItemModel> storage;
        private readonly MainMenuItems mainMenu;

        public MenuItemService(Navigator navigator, ApiClient apiClient)
        {
            Ensure.NotNull(navigator, "navigator");

            storage = new List<MenuItemModel>()
            {
                new MenuItemModel()
                {
                    Identifier = "main-menu",
                    Icon = "bars",
                    Text = "Main menu",
                    OnClick = navigator.ToggleMainMenu,
                    IsRequired = true
                },
                new MenuItemModel()
                {
                    Identifier = "summary-month",
                    Icon = "chart-pie",
                    Text = "Monthly",
                    Url = navigator.UrlSummary(),
                    PageType = typeof(SummaryMonth),
                    IsBlurMenuAfterClick = true
                },
                new MenuItemModel()
                {
                    Identifier = "balances",
                    Icon = "chart-bar",
                    Text = "Balances",
                    Url = navigator.UrlBalances(new YearModel(DateTime.Today.Year)),
                    IsBlurMenuAfterClick = true
                },
                new MenuItemModel()
                {
                    Identifier = "expense-checklist-Month",
                    Icon = "list-check",
                    Text = "Monthly expense checklist",
                    Url = navigator.UrlChecklist(new MonthModel(DateTime.Today.Year, DateTime.Today.Month)),
                    IsBlurMenuAfterClick = true
                },
                new MenuItemModel()
                {
                    Identifier = "search",
                    Icon = "search",
                    Text = "Search",
                    Url = navigator.UrlSearch()
                },
                new MenuItemModel()
                {
                    Identifier = "categories",
                    Icon = "tag",
                    Text = "Categories",
                    Url = navigator.UrlCategories(),
                    IsBlurMenuAfterClick = true
                },
                new MenuItemModel()
                {
                    Identifier = "expensetemplates",
                    Icon = "redo",
                    Text = "Templates",
                    Url = navigator.UrlExpenseTemplates(),
                    IsBlurMenuAfterClick = true
                },
                new MenuItemModel()
                {
                    Identifier = "expense-create",
                    Icon = "minus-circle",
                    Text = "New Expense",
                    OnClick = navigator.OpenExpenseCreate
                }
            };

            mainMenu = new MainMenuItems(
                [
                    new(
                        Text: "Monthly",
                        Icon: "chart-pie",
                        Url: navigator.UrlSummary(),
                        PageType: typeof(Pages.SummaryMonth)
                    ),
                    new(
                        Text: "Yearly",
                        Icon: "circle",
                        Url: navigator.UrlSummary(ThisYear),
                        PageType: typeof(Pages.SummaryYear)
                    ),
                    new(
                        Text: "Trends",
                        Icon: "chart-line",
                        Url: navigator.UrlTrends()
                    ),
                    new(
                        Text: "Balances",
                        Icon: "chart-bar",
                        Url: navigator.UrlBalances(ThisYear),
                        PageType: typeof(Pages.BalancesMonth)
                    ),
                    new(
                        Text: "Search",
                        Icon: "search",
                        Url: navigator.UrlSearch()
                    )
                ],
                [
                    new(
                        Text: "Categories",
                        Icon: "tag",
                        Url: navigator.UrlCategories()
                    ),
                    new(
                        Text: "Currencies",
                        Icon: "pound-sign",
                        Url: navigator.UrlCurrencies()
                    ),
                    new(
                        Text: "Templates",
                        Icon: "redo",
                        Url: navigator.UrlExpenseTemplates()
                    ),
                    new(
                        Text: "About",
                        Icon: "info-circle",
                        Url: navigator.UrlAbout()
                    )
                ],
                [
                    new(
                        Text: "Profile",
                        Icon: "address-card",
                        Url: navigator.UrlUserProfile()
                    ),
                    new(
                        Text: "Password",
                        Icon: "key",
                        Url: navigator.UrlUserPassword()
                    ),
                    new(
                        Text: "Settings",
                        Icon: "cog",
                        Url: navigator.UrlUserSettings()
                    ),
                    new(
                        Text: "Logout",
                        Icon: "sign-out-alt",
                        OnClick: async () => await apiClient.LogoutAsync()
                    )
                ]
            );
        }

        async Task<object> HttpQueryDispatcher.IMiddleware.ExecuteAsync(object query, HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next)
        {
            if (query is ListBottomMenuItem)
            {
                var value = await dispatcher.QueryAsync(new FindUserProperty("MobileMenu"));
                var selectedItems = value != null ? value.Split(',') : Array.Empty<string>();
                return storage.Where(m => selectedItems.Contains(m.Identifier)).ToList<IActionMenuItemModel>();
            }
            else if (query is ListAvailableMenuItem)
            {
                return storage.ToList<IAvailableMenuItemModel>();
            }
            else if (query is ListMainMenuItem)
            {
                return mainMenu;
            }
            else if (query is FindUserProperty findProperty && findProperty.Key == "MobileMenu")
            {
                var value = (string)await next(findProperty);
                if (value == null)
                    return DefaultValue;

                if (!value.Contains("main-menu"))
                    value = $"main-menu,{value}";

                return value;
            }
            else if (query is ListUserProperty listProperty)
            {
                var value = (List<UserPropertyModel>)await next(listProperty);
                var property = value.FirstOrDefault(p => p.Key == "MobileMenu");

                if (property != null && property.Value == null)
                    property.Value = DefaultValue;

                if (!property.Value.Contains("main-menu"))
                    property.Value = $"main-menu,{property.Value}";

                return value;
            }

            return await next(query);
        }
    }
}
