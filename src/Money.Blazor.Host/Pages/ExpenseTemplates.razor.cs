using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Components.Bootstrap;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Queries;
using Money.Services;
using Neptuo;
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
    public partial class ExpenseTemplates : System.IDisposable, 
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
        protected SortDescriptor<ExpenseTemplateSortType> SortDescriptor { get; set; }
        
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

            SortDescriptor = await Queries.QueryAsync(new GetExpenseTemplateSortProperty());

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
        
        protected async void OnSortChanged()
        {
            int Compare<T>(SortDirection direction, T a, T b, Func<T, T, int> comparer) => direction switch
            {
                SortDirection.Ascending => comparer(a, b),
                SortDirection.Descending => comparer(b, a),
                _ => throw new NotSupportedException($"Missing case for {direction}")
            };

            switch (SortDescriptor.Type)
            {
                case ExpenseTemplateSortType.ByAmount:
                    Models.Sort((a, b) => Compare(SortDescriptor.Direction, a.Amount?.Value ?? 0, b.Amount?.Value ?? 0, Decimal.Compare));
                    break;
                case ExpenseTemplateSortType.ByDescription:
                    Models.Sort((a, b) => Compare(SortDescriptor.Direction, a.Description, b.Description, String.Compare));
                    break;
                case ExpenseTemplateSortType.ByCategory:
                    var categoryNames = (await Queries.QueryAsync(ListAllCategory.WithDeleted)).ToDictionary(c => c.Key, c => c.Name);
                    categoryNames[KeyFactory.Empty(typeof(Category))] = String.Empty;
                    Models.Sort((a, b) => 
                    {
                        var result = Compare(SortDescriptor.Direction, categoryNames[a.CategoryKey], categoryNames[b.CategoryKey], String.Compare);
                        if (result == 0)
                            return Compare(SortDescriptor.Direction, a.Description, b.Description, String.Compare);

                        return result;
                    });
                    break;
            }

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