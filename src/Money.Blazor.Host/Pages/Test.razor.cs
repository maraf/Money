using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo;
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
    public partial class Test : System.IDisposable, IEventHandler<ExpenseTemplateCreated>, IEventHandler<ExpenseTemplateDeleted>
    {
        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IEventHandlerCollection EventHandlers { get; set; }

        protected List<ExpenseTemplateModel> ExpenseTemplates { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadAsync();

            EventHandlers.Add<ExpenseTemplateCreated>(this);
            EventHandlers.Add<ExpenseTemplateDeleted>(this);
        }

        public void Dispose()
        {
            EventHandlers.Remove<ExpenseTemplateCreated>(this);
            EventHandlers.Remove<ExpenseTemplateDeleted>(this);
        }

        private async Task LoadAsync()
        {
            ExpenseTemplates = await Queries.QueryAsync(new ListAllExpenseTemplate());
        }

        protected async Task CreateAsync()
        {
            await Commands.HandleAsync(new CreateExpenseTemplate(new Price(2500, "CZK"), "2.5K", KeyFactory.Empty(typeof(Category))));
        }

        public async Task HandleAsync(ExpenseTemplateCreated payload)
        {
            await LoadAsync();
            StateHasChanged();
        }

        public async Task HandleAsync(ExpenseTemplateDeleted payload)
        {
            await LoadAsync();
            StateHasChanged();
        }
    }
}
