using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Pages;
using Money.Services;
using Neptuo.Queries;
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

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        protected List<IActionMenuItemModel> Items { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Items = await Queries.QueryAsync(new ListBottomMenuItem());
        }
    }
}
