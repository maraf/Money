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

        private readonly List<MenuItemModel> storage;

        public MenuItemService(Navigator navigator)
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
