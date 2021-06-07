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
        private const string DefaultValue = "summary-month,expense-create";

        private readonly Navigator navigator;
        private readonly List<MenuItemModel> storage;

        public MenuItemService(Navigator navigator)
        {
            Ensure.NotNull(navigator, "navigator");
            this.navigator = navigator;

            storage = new List<MenuItemModel>()
            {
                new MenuItemModel()
                {
                    Identifier = "summary-month",
                    Icon = "chart-pie",
                    Text = "Monthly",
                    Url = navigator.UrlSummary(),
                    PageType = typeof(SummaryMonth)
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
                    Url = navigator.UrlCategories()
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
                
                Console.WriteLine($"MM: (1) '{value}', {value == null}, {value == String.Empty}");

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

                Console.WriteLine($"MM: (2) '{value}', {value == null}, {value == String.Empty}");

                if (value == null)
                    return DefaultValue;

                return value;
            }
            else if (query is ListUserProperty listProperty)
            {
                var value = (List<UserPropertyModel>)await next(listProperty);
                var property = value.FirstOrDefault(p => p.Key == "MobileMenu");

                Console.WriteLine($"MM: (3) '{property.Value}', {property.Value == null}, {property.Value == String.Empty}");
                if (property != null && property.Value == null)
                    property.Value = DefaultValue;

                return value;
            }

            return await next(query);
        }
    }
}
