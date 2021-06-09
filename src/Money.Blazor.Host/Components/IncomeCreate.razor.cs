using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class IncomeCreate
    {
        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        internal ILog<IncomeCreate> Log { get; set; }

        [Inject]
        internal Navigator Navigator { get; set; }

        protected string Title { get; set; }
        protected string SaveButtonText { get; set; }
        protected List<string> ErrorMessages { get; } = new List<string>();

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

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            Title = "Create a new Income";
            SaveButtonText = "Create";

            Currencies = await Queries.QueryAsync(new ListAllCurrency());
            Currency = await Queries.QueryAsync(new FindCurrencyDefault());
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
            Log.Debug($"Income: Amount: {Amount}, Currency: {Currency}, When: {When}.");

            ErrorMessages.Clear();
            Validator.AddIncomeAmount(ErrorMessages, Amount);
            Validator.AddIncomeDescription(ErrorMessages, Description);
            Validator.AddIncomeCurrency(ErrorMessages, Currency);

            Log.Debug($"Income: Validation: {string.Join(", ", ErrorMessages)}.");
            return ErrorMessages.Count == 0;
        }

        private async void Execute()
        {
            await Commands.HandleAsync(new CreateIncome(new Price(Amount, Currency), Description, When));

            Amount = 0;
            Description = null;
            StateHasChanged();
        }

        public override void Show()
        {
            if (Currencies == null || Currencies.Count == 0)
                PrerequisitesConfirm.Show();
            else
                base.Show();
        }

        protected void OnPrerequisitesConfirmed()
        {
            if (Currencies == null || Currencies.Count == 0)
                Navigator.OpenCurrencies();
        }
    }
}
