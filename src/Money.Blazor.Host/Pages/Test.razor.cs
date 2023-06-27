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
    public partial class Test : System.IDisposable, 
        IEventHandler<ExpenseTemplateCreated>,
        IEventHandler<ExpenseTemplateAmountChanged>,
        IEventHandler<ExpenseTemplateDescriptionChanged>,
        IEventHandler<ExpenseTemplateCategoryChanged>,
        IEventHandler<ExpenseTemplateFixedChanged>,
        IEventHandler<ExpenseTemplateRecurrenceChanged>,
        IEventHandler<ExpenseTemplateRecurrenceCleared>,
        IEventHandler<ExpenseTemplateDeleted>
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
            EventHandlers.Add<ExpenseTemplateAmountChanged>(this);
            EventHandlers.Add<ExpenseTemplateDescriptionChanged>(this);
            EventHandlers.Add<ExpenseTemplateCategoryChanged>(this);
            EventHandlers.Add<ExpenseTemplateFixedChanged>(this);
            EventHandlers.Add<ExpenseTemplateDeleted>(this);
        }

        public void Dispose()
        {
            EventHandlers.Remove<ExpenseTemplateCreated>(this);
            EventHandlers.Remove<ExpenseTemplateAmountChanged>(this);
            EventHandlers.Remove<ExpenseTemplateDescriptionChanged>(this);
            EventHandlers.Remove<ExpenseTemplateCategoryChanged>(this);
            EventHandlers.Remove<ExpenseTemplateFixedChanged>(this);
            EventHandlers.Remove<ExpenseTemplateDeleted>(this);
        }

        private async Task LoadAsync()
        {
            ExpenseTemplates = await Queries.QueryAsync(ListAllExpenseTemplate.Version3());
        }

        protected async Task CreateAsync()
        {
            await Commands.HandleAsync(new CreateExpenseTemplate(new Price(2500, "CZK"), "2.5K", KeyFactory.Empty(typeof(Category))));
        }

        public async Task HandleAsync(ExpenseTemplateCreated payload)
        {
            Console.WriteLine("ExpenseTemplateCreated");
            await LoadAsync();
            StateHasChanged();
        }

        public async Task HandleAsync(ExpenseTemplateDeleted payload)
        {
            Console.WriteLine("ExpenseTemplateDeleted");
            await LoadAsync();
            StateHasChanged();
        }

        public async Task HandleAsync(ExpenseTemplateAmountChanged payload)
        {
            Console.WriteLine("ExpenseTemplateAmountChanged");
            await LoadAsync();
            StateHasChanged();
        }

        public async Task HandleAsync(ExpenseTemplateDescriptionChanged payload)
        {
            Console.WriteLine("ExpenseTemplateDescriptionChanged");
            await LoadAsync();
            StateHasChanged();
        }

        public async Task HandleAsync(ExpenseTemplateCategoryChanged payload)
        {
            Console.WriteLine("ExpenseTemplateCategoryChanged");
            await LoadAsync();
            StateHasChanged();
        }

        public async Task HandleAsync(ExpenseTemplateFixedChanged payload)
        {
            Console.WriteLine("ExpenseTemplateFixedChanged");
            await LoadAsync();
            StateHasChanged();
        }

        public async Task HandleAsync(ExpenseTemplateRecurrenceChanged payload)
        {
            Console.WriteLine("ExpenseTemplateRecurrenceChanged");
            await LoadAsync();
            StateHasChanged();
        }

        public async Task HandleAsync(ExpenseTemplateRecurrenceCleared payload)
        {
            Console.WriteLine("ExpenseTemplateRecurrenceCleared");
            await LoadAsync();
            StateHasChanged();
        }
    }
}
