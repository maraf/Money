using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class Trends
    {
        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected Navigator Navigator { get; set; }

        protected List<CategoryModel> Categories { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Categories = await Queries.QueryAsync(new ListAllCategory());
        }
    }
}
