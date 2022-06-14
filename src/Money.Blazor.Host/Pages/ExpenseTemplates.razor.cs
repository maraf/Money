using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using Neptuo.Queries.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class ExpenseTemplates : IDisposable, IEventHandler<ExpenseTemplateCreated>, IEventHandler<ExpenseTemplateDeleted>
    {
        [Inject]
        protected IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }

        protected CurrencyFormatter CurrencyFormatter { get; private set; }
        protected ExpenseTemplateCreate Modal { get; set; }
        protected List<ExpenseTemplateModel> Models { get; } = new List<ExpenseTemplateModel>();
        protected List<CategoryModel> Categories { get; private set; }

        protected async override Task OnInitializedAsync()
        {
            EventHandlers
                .Add<ExpenseTemplateCreated>(this)
                .Add<ExpenseTemplateDeleted>(this);

            await base.OnInitializedAsync();

            Categories = await Queries.QueryAsync(new ListAllCategory(true));
            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();

            await ReloadAsync();
        }

        protected string FindCategoryName(IKey categoryKey)
        {
            var category = Categories.FirstOrDefault(c => c.Key.Equals(categoryKey));
            return category?.Name;
        }

        protected Color? FindCategoryColor(IKey categoryKey)
        {
            var category = Categories.FirstOrDefault(c => c.Key.Equals(categoryKey));
            return category?.Color;
        }

        private async Task ReloadAsync()
        {
            Models.Clear();
            Models.AddRange(await Queries.QueryAsync(new ListAllExpenseTemplate()));
            StateHasChanged();
        }

        public void Dispose() => EventHandlers
                .Remove<ExpenseTemplateCreated>(this)
                .Remove<ExpenseTemplateDeleted>(this);

        public Task HandleAsync(ExpenseTemplateCreated payload)
        {
            _ = ReloadAsync();
            return Task.CompletedTask;
        }

        public Task HandleAsync(ExpenseTemplateDeleted payload)
        {
            _ = ReloadAsync();
            return Task.CompletedTask;
        }
    }
}