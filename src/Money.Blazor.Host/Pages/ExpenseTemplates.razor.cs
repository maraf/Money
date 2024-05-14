using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Components.Bootstrap;
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
        IEventHandler<ExpenseTemplateAmountChanged>,
        IEventHandler<ExpenseTemplateDescriptionChanged>,
        IEventHandler<ExpenseTemplateCategoryChanged>,
        IEventHandler<ExpenseTemplateRecurrenceChanged>,
        IEventHandler<ExpenseTemplateRecurrenceCleared>,
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
        protected ExpenseTemplateAmount ChangeAmountModal { get; set; }
        protected ExpenseTemplateCategory ChangeCategoryModal { get; set; }
        protected ExpenseTemplateRecurrence ChangeRecurrenceModal { get; set; }
        protected Confirm DeleteConfirm { get; set; }
        protected OutcomeCreate ExpenseModal { get; set; }
        protected List<ExpenseTemplateModel> Models { get; } = new List<ExpenseTemplateModel>();
        
        protected IKey ToDeleteKey { get; set; }
        protected ExpenseTemplateModel Selected { get; set; }
        protected string DeleteMessage { get; set; }

        protected async override Task OnInitializedAsync()
        {
            EventHandlers
                .Add<ExpenseTemplateCreated>(this)
                .Add<ExpenseTemplateAmountChanged>(this)
                .Add<ExpenseTemplateDescriptionChanged>(this)
                .Add<ExpenseTemplateCategoryChanged>(this)
                .Add<ExpenseTemplateRecurrenceChanged>(this)
                .Add<ExpenseTemplateRecurrenceCleared>(this)
                .Add<ExpenseTemplateDeleted>(this);

            await base.OnInitializedAsync();

            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();

            await ReloadAsync();
        }

        public void Dispose() => EventHandlers
            .Remove<ExpenseTemplateCreated>(this)
            .Remove<ExpenseTemplateAmountChanged>(this)
            .Remove<ExpenseTemplateDescriptionChanged>(this)
            .Remove<ExpenseTemplateCategoryChanged>(this)
            .Remove<ExpenseTemplateRecurrenceChanged>(this)
            .Remove<ExpenseTemplateRecurrenceCleared>(this)
            .Remove<ExpenseTemplateDeleted>(this);

        protected string DayInMonth(int day) => day switch
        {
            1 => "1st",
            2 => "2nd",
            3 => "3rd",
            _ => $"{day}th"
        };

        private async Task ReloadAsync()
        {
            Models.Clear();
            Models.AddRange(await Queries.QueryAsync(ListAllExpenseTemplate.Version3()));
            StateHasChanged();
        }

        protected void Delete()
        {
            _ = Commands.HandleAsync(new DeleteExpenseTemplate(ToDeleteKey));
        }

        protected void Edit(ExpenseTemplateModel selected, ModalDialog modal)
        {
            Selected = selected; 
            modal.Show(); 
            StateHasChanged();
        }

        protected Task OnEventAsync()
        {
            _ = ReloadAsync();
            return Task.CompletedTask;
        }

        public Task HandleAsync(ExpenseTemplateCreated payload) => OnEventAsync();
        public Task HandleAsync(ExpenseTemplateDescriptionChanged payload) => OnEventAsync();
        public Task HandleAsync(ExpenseTemplateDeleted payload) => OnEventAsync();
        public Task HandleAsync(ExpenseTemplateAmountChanged payload) => OnEventAsync();
        public Task HandleAsync(ExpenseTemplateCategoryChanged payload) => OnEventAsync();
        public Task HandleAsync(ExpenseTemplateRecurrenceChanged payload) => OnEventAsync();
        public Task HandleAsync(ExpenseTemplateRecurrenceCleared payload) => OnEventAsync();
    }
}