using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Commands;
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
    public partial class OutcomeAmount
    {
        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        private decimal originalAmount;
        private string originalCurrency;

        public List<CurrencyModel> Currencies { get; private set; }
        protected List<string> ErrorMessages { get; } = new List<string>();

        [Parameter]
        public IKey OutcomeKey { get; set; }

        [Parameter]
        public decimal Amount { get; set; }

        [Parameter]
        public string Currency { get; set; }

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            SetOriginal();
            Currencies = await Queries.QueryAsync(new ListAllCurrency());
        }

        private void SetOriginal()
        {
            originalAmount = Amount;
            originalCurrency = Currency;
        }

        protected void OnSaveClick()
        {
            if (Validate() && (originalAmount != Amount || originalCurrency != Currency))
            {
                Execute();
                SetOriginal();
                Modal.Hide();
            }
        }

        private bool Validate()
        {
            ErrorMessages.Clear();
            Validator.AddOutcomeAmount(ErrorMessages, Amount);
            Validator.AddOutcomeCurrency(ErrorMessages, Currency);

            return ErrorMessages.Count == 0;
        }

        private async void Execute()
        {
            await Commands.HandleAsync(new ChangeOutcomeAmount(OutcomeKey, new Price(Amount, Currency)));
        }
    }
}
