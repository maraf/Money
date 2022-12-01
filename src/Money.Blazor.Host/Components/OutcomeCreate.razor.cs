using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Logging;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class OutcomeCreate : System.IDisposable
    {
        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        internal ILog<OutcomeCreate> Log { get; set; }

        [Inject]
        internal Navigator Navigator { get; set; }

        [Inject]
        internal Interop Interop { get; set; }

        [Inject]
        protected Navigator.ComponentContainer ComponentContainer { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }

        protected CurrencyFormatter CurrencyFormatter { get; private set; }

        protected string Title { get; set; }
        protected string SaveButtonText { get; set; }
        protected List<string> ErrorMessages { get; } = new List<string>();

        protected List<ExpenseTemplateModel> Templates { get; private set; }
        protected List<CategoryModel> Categories { get; private set; }
        protected List<CurrencyModel> Currencies { get; private set; }

        protected Confirm PrerequisitesConfirm { get; set; }

        [Parameter]
        public string Description { get; set; }

        [Parameter]
        public DateTime When { get; set; } = DateTime.UtcNow.Date;

        [Parameter]
        public IKey CategoryKey { get; set; }

        [Parameter]
        public bool IsFixed { get; set; }

        [Parameter]
        public Price Amount { get; set; }

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

        public void Dispose()
        {
            if (isAttachedToComponentContainer)
                ComponentContainer.ExpenseCreate = null;
        }

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            Title = "Create a new Expense";
            SaveButtonText = "Create";
        }

        protected async Task OnSaveClickAsync()
        {
            if (await Validate(ErrorMessages))
            {
                Execute();
                Modal.Hide();
            }
        }

        private async Task<bool> Validate(List<string> messages)
        {
            Log.Debug($"Expense: Amount: {Amount}, Category: {CategoryKey}, When: {When}.");

            messages.Clear();
            if (messages.Count == 0 && (Amount == null || Validator.AddOutcomeAmount(messages, Amount.Value)))
                await FocusElementAsync("expense-amount");

            if (messages.Count == 0 && Validator.AddOutcomeDescription(messages, Description))
                await FocusElementAsync("expense-description");

            if (messages.Count == 0 && Validator.AddOutcomeCategoryKey(messages, CategoryKey))
                await FocusElementAsync("expense-category-first");

            Validator.AddOutcomeCurrency(messages, Amount == null ? null : Amount.Currency);

            Log.Debug($"Expense: Validation: {string.Join(", ", messages)}.");
            return messages.Count == 0;
        }

        private Task FocusElementAsync(string id) 
            => Interop.FocusElementByIdAsync(id);

        private async void Execute()
        {
            await Commands.HandleAsync(new CreateOutcome(Amount, Description, When, CategoryKey, IsFixed));

            Amount = null;
            CategoryKey = null;
            Description = null;
            IsFixed = false;
            StateHasChanged();
        }

        public async void Show(IKey categoryKey)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            CategoryKey = categoryKey;

            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();

            Categories = await Queries.QueryAsync(new ListAllCategory());
            Currencies = await Queries.QueryAsync(new ListAllCurrency());
            Templates = await Queries.QueryAsync(new ListAllExpenseTemplate());

            if (Currencies == null || Currencies.Count == 0 || Categories == null || Categories.Count == 0)
                PrerequisitesConfirm.Show();
            else
                base.Show();

            AreTemplatesOpened = false;
            StateHasChanged();
        }

        public void Show(Price amount, string description, IKey categoryKey)
        {
            if (amount != null)
                Amount = amount;

            if (!String.IsNullOrEmpty(description))
                Description = description;

            Show(categoryKey);
        }

        public override void Show()
            => Show(KeyFactory.Empty(typeof(Category)));

        protected void OnPrerequisitesConfirmed()
        {
            if (Currencies == null || Currencies.Count == 0)
                Navigator.OpenCurrencies();
            else if (Categories == null || Categories.Count == 0)
                Navigator.OpenCategories();
        }

        protected async Task ApplyTemplateAsync(ExpenseTemplateModel model)
        {
            if (model.Amount != null)
                Amount = model.Amount;

            if (!String.IsNullOrEmpty(model.Description))
                Description = model.Description;

            if (!model.CategoryKey.IsEmpty)
                CategoryKey = model.CategoryKey;

            IsFixed = model.IsFixed;
            AreTemplatesOpened = false;

            await Validate(new List<string>());
        }

        protected string FindCategoryName(IKey categoryKey)
        {
            var category = Categories?.FirstOrDefault(c => c.Key.Equals(categoryKey));
            return category?.Name;
        }

        protected Color? FindCategoryColor(IKey categoryKey)
        {
            var category = Categories?.FirstOrDefault(c => c.Key.Equals(categoryKey));
            return category?.Color;
        }
    }
}
