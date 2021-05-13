using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class Test
    {
        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IEventHandlerCollection EventHandlers { get; set; }

        protected List<CategoryModel> Categories { get; set; }
        protected List<YearWithAmountModel> Summaries { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Categories = await Queries.QueryAsync(new ListAllCategory());
        }

        protected async Task LoadCategoryAsync(IKey categoryKey)
        {
            Summaries = null;
            Summaries = await Queries.QueryAsync(new ListYearOutcomesForCategory(categoryKey, new YearModel(2015)));
        }
    }
}
