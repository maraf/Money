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
    public partial class OutcomeCreate
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
        public decimal Amount { get; set; }

        [Parameter]
        public string Currency { get; set; }

        [Parameter]
        public string Description { get; set; }

        [Parameter]
        public DateTime When { get; set; } = DateTime.UtcNow.Date;

        [Parameter]
        public IKey CategoryKey { get; set; }

        [Parameter]
        public bool IsFixed { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ComponentContainer.ExpenseCreate = this;
        }

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            Title = "Create a new Expense";
            SaveButtonText = "Create";
        }

        protected async Task OnSaveClickAsync()
        {
            if (await Validate())
            {
                Execute();
                Modal.Hide();
            }
        }

        private async Task<bool> Validate()
        {
            Log.Debug($"Expense: Amount: {Amount}, Currency: {Currency}, Category: {CategoryKey}, When: {When}.");

            ErrorMessages.Clear();
            if (ErrorMessages.Count == 0 && Validator.AddOutcomeAmount(ErrorMessages, Amount))
                await FocusElementAsync("expense-amount");

            if (ErrorMessages.Count == 0 && Validator.AddOutcomeDescription(ErrorMessages, Description))
                await FocusElementAsync("expense-description");

            if (ErrorMessages.Count == 0 && Validator.AddOutcomeCategoryKey(ErrorMessages, CategoryKey))
                await FocusElementAsync("expense-category-first");

            Validator.AddOutcomeCurrency(ErrorMessages, Currency);

            Log.Debug($"Expense: Validation: {string.Join(", ", ErrorMessages)}.");
            return ErrorMessages.Count == 0;
        }

        private Task FocusElementAsync(string id) 
            => Interop.FocusElementByIdAsync(id);

        private async void Execute()
        {
            await Commands.HandleAsync(new CreateOutcome(new Price(Amount, Currency), Description, When, CategoryKey, IsFixed));

            Amount = 0;
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
            Currency = await Queries.QueryAsync(new FindCurrencyDefault());

            if (Currencies == null || Currencies.Count == 0 || Categories == null || Categories.Count == 0)
                PrerequisitesConfirm.Show();
            else
                base.Show();

            StateHasChanged();
        }

        public async void Show(decimal? amount, string currency, string description, IKey categoryKey)
        {
            if (amount != null)
                Amount = amount.Value;

            if (!String.IsNullOrEmpty(currency))
                Currency = currency;

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

        protected void ApplyTemplate(ExpenseTemplateModel model)
        {
            if (model.Amount != null)
            {
                Amount = model.Amount.Value;
                Currency = model.Amount.Currency;
            }

            if (!String.IsNullOrEmpty(model.Description))
                Description = model.Description;

            if (!model.CategoryKey.IsEmpty)
                CategoryKey = model.CategoryKey;
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
