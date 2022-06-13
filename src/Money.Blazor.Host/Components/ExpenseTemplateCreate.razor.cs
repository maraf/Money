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
        public decimal Amount { get; set; }

        [Parameter]
        public string Currency { get; set; }

        [Parameter]
        public string Description { get; set; }

        [Parameter]
        public IKey CategoryKey { get; set; }

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
            Log.Debug($"Expense: Amount: {Amount}, Currency: {Currency}, Category: {CategoryKey}.");

            ErrorMessages.Clear();

            if ((Amount == 0 && Currency != null) || (Amount != 0 && Currency == null))
                ErrorMessages.Add("Amount and currency must be provided both or none");

            if (Amount != 0)
                Validator.AddOutcomeAmount(ErrorMessages, Amount);

            Log.Debug($"Expense: Validation: {string.Join(", ", ErrorMessages)}.");
            return ErrorMessages.Count == 0;
        }

        private async void Execute()
        {
            var price = Amount != 0
                ? new Price(Amount, Currency)
                : null;

            await Commands.HandleAsync(new CreateExpenseTemplate(price, Description, CategoryKey));

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
                base.Show();

            StateHasChanged();
        }

        public override void Show()
            => Show(KeyFactory.Empty(typeof(Category)));
    }
}
