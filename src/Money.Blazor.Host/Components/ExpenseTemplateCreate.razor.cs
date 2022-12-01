using Microsoft.AspNetCore.Components;
using Money.Commands;
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
    public partial class ExpenseTemplateCreate
    {
        public const string EmptyCurrency = "---";

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        internal ILog<ExpenseTemplateCreate> Log { get; set; }

        protected List<string> ErrorMessages { get; } = new List<string>();

        protected List<CategoryModel> Categories { get; private set; }
        protected List<CurrencyModel> Currencies { get; private set; }

        protected Confirm PrerequisitesConfirm { get; set; }

        [Parameter]
        public Price Amount { get; set; }

        [Parameter]
        public string Description { get; set; }

        [Parameter]
        public IKey CategoryKey { get; set; }

        [Parameter]
        public bool IsFixed { get; set; }

        protected Price Price { get; set; }

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
            Log.Debug($"Expense: Amount: {Amount}, Category: {CategoryKey}.");

            ErrorMessages.Clear();

            if (Amount != null && ((Amount.Value == 0 && Amount.Currency != null) || (Amount.Value != 0 && Amount.Currency == null)))
                ErrorMessages.Add("Amount and currency must be provided both or none");

            if (Amount != null && Amount.Value != 0)
                Validator.AddOutcomeAmount(ErrorMessages, Amount.Value);

            Log.Debug($"Expense: Validation: {string.Join(", ", ErrorMessages)}.");
            return ErrorMessages.Count == 0;
        }

        private async void Execute()
        {
            await Commands.HandleAsync(new CreateExpenseTemplate(Amount, Description, CategoryKey, IsFixed));

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

            Categories = await Queries.QueryAsync(new ListAllCategory());
            Currencies = await Queries.QueryAsync(new ListAllCurrency());

            if (Currencies == null || Currencies.Count == 0 || Categories == null || Categories.Count == 0)
                PrerequisitesConfirm.Show();
            else
                base.Show();

            StateHasChanged();
        }

        public void Show(Price amount, string description, IKey categoryKey)
        {
            Amount = amount;
            Description = description;

            Show(categoryKey);
        }

        public override void Show()
            => Show(KeyFactory.Empty(typeof(Category)));
    }
}
