using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Pages;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class CurrencyEdit
    {
        private string originalUniqueCode;
        private string originalSymbol;
        protected List<string> ErrorMessages { get; } = new List<string>();

        [Inject]
        internal ILog<CurrencyEdit> Log { get; set; }

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Parameter]
        public string UniqueCode { get; set; }

        [Parameter]
        public string Symbol { get; set; }

        protected string Title { get; set; }
        protected string SaveButtonText { get; set; }
        protected bool IsUniqueCodeEnabled { get; set; }

        private bool isNew;

        public override Task SetParametersAsync(ParameterView parameters)
        {
            isNew = parameters.GetValueOrDefault<string>(nameof(UniqueCode)) == null;

            return base.SetParametersAsync(parameters);
        }

        protected override Task OnParametersSetAsync()
        {
            originalUniqueCode = UniqueCode;
            originalSymbol = Symbol;

            if (isNew)
            {
                Title = "Create a new Currency";
                SaveButtonText = "Create";
                IsUniqueCodeEnabled = true;
            }
            else
            {
                Title = "Change Currency";
                SaveButtonText = "Save";
                IsUniqueCodeEnabled = false;
            }

            return base.OnParametersSetAsync();
        }

        protected void OnSaveClick()
        {
            if (isNew)
            {
                if (Validate())
                {
                    ExecuteCreate();
                    Modal.Hide();
                }
            }
            else if (originalSymbol != Symbol)
            {
                ExecuteChange();
                originalSymbol = Symbol;
                Modal.Hide();
            }
        }

        private bool Validate()
        {
            ErrorMessages.Clear();
            Validator.AddCurrencyUniqueCode(ErrorMessages, UniqueCode);
            Validator.AddCurrencySymbol(ErrorMessages, Symbol);

            return ErrorMessages.Count == 0;
        }

        private async void ExecuteCreate()
        {
            await Commands.HandleAsync(new CreateCurrency(UniqueCode, Symbol));
            UniqueCode = Symbol = String.Empty;
            StateHasChanged();
        }

        private async void ExecuteChange()
        {
            await Commands.HandleAsync(new ChangeCurrencySymbol(UniqueCode, Symbol));
        }

        private void SetUsd() => SetValue("USD", "$");
        private void SetEur() => SetValue("EUR", "€");
        private void SetCzk() => SetValue("CZK", "Kč");

        private void SetValue(string uniqueCode, string symbol)
        {
            UniqueCode = uniqueCode;
            Symbol = symbol;
        }
    }
}
