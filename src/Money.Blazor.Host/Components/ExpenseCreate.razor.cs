using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Pages;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class ExpenseCreate : System.IDisposable, IExpenseCreate
    {
        protected IKey EmptyCategoryKey { get; } = KeyFactory.Empty(typeof(Category));

        [Inject]
        protected Interop Interop { get; set; }

        [Inject]
        protected Navigator Navigator { get; set; }

        [Inject]
        protected Navigator.ComponentContainer ComponentContainer { get; set; }

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }
        protected CurrencyFormatter CurrencyFormatter { get; set; }

        protected List<ExpenseTemplateModel> Templates { get; private set; }
        protected List<CategoryModel> Categories { get; private set; }
        protected List<CurrencyModel> Currencies { get; private set; }

        protected SelectedField Selected { get; set; } = SelectedField.Description;
        protected Price Amount { get; set; }
        protected string Description { get; set; }
        protected List<ExpenseTemplateModel> SuggestedTemplates { get; set; } = new();
        protected IKey CategoryKey { get; set; } = KeyFactory.Empty(typeof(Category));
        protected DateTime When { get; set; }

        protected bool AreTemplatesOpened { get; set; }
        private bool isAttachedToComponentContainer;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (ComponentContainer.ExpenseCreate == null)
            {
                ComponentContainer.ExpenseCreate = this;
                isAttachedToComponentContainer = true;
            }
        }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            string elementId = Selected switch 
            {
                SelectedField.Description => "expense-wiz-description",
                SelectedField.Amount => "expense-wiz-amount",
                SelectedField.Category => !CategoryKey.IsEmpty ? $"expense-wiz-category-{CategoryKey.AsGuidKey().Guid.ToString()}" : null,
                SelectedField.When => "expense-wiz-when",
                _ => null,
            };

            System.Console.WriteLine($"DEBUG: element to focus '{elementId}'");

            if (!String.IsNullOrEmpty(elementId))
                await Interop.FocusElementByIdAsync(elementId);
        }

        public void Dispose()
        {
            if (isAttachedToComponentContainer)
                ComponentContainer.ExpenseCreate = null;
        }

        public void Show(IKey categoryKey)
        {
            base.Show();
            CategoryKey = categoryKey;

            _ = LoadAsync();
        }

        public void Show(Price amount, string description, IKey categoryKey, bool isFixed)
        {
            base.Show();
            Amount = amount;
            Description = description;
            CategoryKey = categoryKey;
            // TODO: isFixed

            _ = LoadAsync();
        }

        protected async Task LoadAsync()
        {
            if (When == DateTime.MinValue)
                When = DateTime.Today;

            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();
            Categories = await Queries.QueryAsync(new ListAllCategory());
            Currencies = await Queries.QueryAsync(new ListAllCurrency());
            Templates = await Queries.QueryAsync(new ListAllExpenseTemplate());
            StateHasChanged();
        }

        protected enum SelectedField
        {
            Amount,
            Description,
            Category,
            When
        }

        protected void SuggestTemplates()
        {
            AreTemplatesOpened = true;
            SuggestedTemplates.Clear();
            string description = Description?.ToLowerInvariant();
            if (String.IsNullOrEmpty(description))
                return;

            foreach (var template in Templates)
            {
                string templateDescription = template.Description?.ToLowerInvariant();
                if (String.IsNullOrEmpty(description))
                    continue;

                if (templateDescription.Contains(description))
                    SuggestedTemplates.Add(template);
            }
        }

        protected Task ApplyTemplateAsync(ExpenseTemplateModel model)
        {
            Amount = model.Amount;
            Description = model.Description;
            CategoryKey = model.CategoryKey;
            //TODO: IsFixed = model.IsFixed;

            SuggestedTemplates.Clear();
            AreTemplatesOpened = false;

            return Task.CompletedTask;
        }
    }
}
