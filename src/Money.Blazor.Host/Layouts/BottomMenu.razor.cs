using Microsoft.AspNetCore.Components;
using Money.Pages;
using Money.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Layouts
{
    public partial class BottomMenu
    {
        [Inject]
        protected Navigator Navigator { get; set; }

        protected List<MenuItemModel> Items { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Items = new List<MenuItemModel>()
            {
                new MenuItemModel()
                {
                    Icon = "chart-pie",
                    Text = "Monthly",
                    Url = Navigator.UrlSummary(),
                    PageType = typeof(SummaryMonth)
                },
                new MenuItemModel()
                {
                    Icon = "search",
                    Text = "Search",
                    Url = Navigator.UrlSearch()
                },
                new MenuItemModel()
                {
                    Icon = "tag",
                    Text = "Categories",
                    Url = Navigator.UrlCategories()
                },
                new MenuItemModel()
                {
                    Icon = "minus-circle",
                    Text = "New Expense",
                    OnClick = Navigator.OpenExpenseCreate
                }
            };
        }
    }

    public class MenuItemModel
    {
        public string Icon { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public Type PageType { get; set; }
        public Action OnClick { get; set; }
    }
}
