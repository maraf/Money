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
        protected Navigator.ModalContainer ModalContainer { get; set; }

        protected Modal Modal { get; set; }

        protected string Title { get; set; }
        protected string SaveButtonText { get; set; }
        protected List<string> ErrorMessages { get; } = new List<string>();

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
            ModalContainer.ExpenseCreate = this;
        }

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            Title = "Create a new Expense";
            SaveButtonText = "Create";
        }

        protected void OnSaveClick()
        {
            if (Validate())
            {
                Execute();
                Modal.Hide();
            }
        }

        private bool Validate()
        {
            Log.Debug($"Expense: Amount: {Amount}, Currency: {Currency}, Category: {CategoryKey}, When: {When}.");

            ErrorMessages.Clear();
            Validator.AddOutcomeAmount(ErrorMessages, Amount);
            Validator.AddOutcomeDescription(ErrorMessages, Description);
            Validator.AddOutcomeCurrency(ErrorMessages, Currency);
            Validator.AddOutcomeCategoryKey(ErrorMessages, CategoryKey);

            Log.Debug($"Expense: Validation: {string.Join(", ", ErrorMessages)}.");
            return ErrorMessages.Count == 0;
        }

        private async void Execute()
        {
            await Commands.HandleAsync(new CreateOutcome(new Price(Amount, Currency), Description, When, CategoryKey, IsFixed));

            Amount = 0;
            CategoryKey = null;
            Description = null;
            StateHasChanged();
        }

        public async void Show(IKey categoryKey)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            CategoryKey = categoryKey;

            Categories = await Queries.QueryAsync(new ListAllCategory());
            Currencies = await Queries.QueryAsync(new ListAllCurrency());
            Currency = await Queries.QueryAsync(new FindCurrencyDefault());

            if (Currencies == null || Currencies.Count == 0 || Categories == null || Categories.Count == 0)
                PrerequisitesConfirm.Show();
            else
                Modal.Show();

            StateHasChanged();
        }

        public void Hide() => Modal.Hide();

        protected void OnPrerequisitesConfirmed()
        {
            if (Currencies == null || Currencies.Count == 0)
                Navigator.OpenCurrencies();
            else if (Categories == null || Categories.Count == 0)
                Navigator.OpenCategories();
        }
    }
}
