using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Commands;
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
    public partial class ExpenseTemplates : IDisposable, 
        IEventHandler<ExpenseTemplateCreated>,
        IEventHandler<ExpenseTemplateDescriptionChanged>,
        IEventHandler<ExpenseTemplateDeleted>
    {
        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected Navigator Navigator { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }

        protected CurrencyFormatter CurrencyFormatter { get; private set; }
        protected ExpenseTemplateCreate CreateModal { get; set; }
        protected ExpenseTemplateDescription ChangeDescriptionModal { get; set; }
        protected Confirm DeleteConfirm { get; set; }
        protected OutcomeCreate ExpenseModal { get; set; }
        protected List<ExpenseTemplateModel> Models { get; } = new List<ExpenseTemplateModel>();
        protected List<CategoryModel> Categories { get; private set; }
        
        protected IKey ToDeleteKey { get; set; }
        protected ExpenseTemplateModel Selected { get; set; }
        protected string DeleteMessage { get; set; }

        protected async override Task OnInitializedAsync()
        {
            EventHandlers
                .Add<ExpenseTemplateCreated>(this)
                .Add<ExpenseTemplateDescriptionChanged>(this)
                .Add<ExpenseTemplateDeleted>(this);

            await base.OnInitializedAsync();

            Categories = await Queries.QueryAsync(new ListAllCategory(true));
            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();

            await ReloadAsync();
        }

        public void Dispose() => EventHandlers
            .Remove<ExpenseTemplateCreated>(this)
            .Remove<ExpenseTemplateDescriptionChanged>(this)
            .Remove<ExpenseTemplateDeleted>(this);

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
            Models.AddRange(await Queries.QueryAsync(ListAllExpenseTemplate.Version2()));
            StateHasChanged();
        }

        protected void Delete()
        {
            _ = Commands.HandleAsync(new DeleteExpenseTemplate(ToDeleteKey));
        }

        protected Task OnEventAsync()
        {
            _ = ReloadAsync();
            return Task.CompletedTask;
        }

        public Task HandleAsync(ExpenseTemplateCreated payload) => OnEventAsync();
        public Task HandleAsync(ExpenseTemplateDescriptionChanged payload) => OnEventAsync();
        public Task HandleAsync(ExpenseTemplateDeleted payload) => OnEventAsync();
    }
}