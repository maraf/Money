using Money.Commands;
using Money.Models;
using Money.ViewModels.Navigation;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Commands
{
    public class CurrencyDeleteExchangeRateCommand : Neptuo.Observables.Commands.Command<ExchangeRateModel>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly INavigator navigator;
        private readonly string targetCurrencyUniqueCode;

        public CurrencyDeleteExchangeRateCommand(ICommandDispatcher commandDispatcher, INavigator navigator, string targetCurrencyUniqueCode)
        {
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNullOrEmpty(targetCurrencyUniqueCode, "targetCurrencyUniqueCode");
            this.commandDispatcher = commandDispatcher;
            this.navigator = navigator;
            this.targetCurrencyUniqueCode = targetCurrencyUniqueCode;
        }

        public override bool CanExecute(ExchangeRateModel parameter)
        {
            return parameter != null;
        }

        public override void Execute(ExchangeRateModel parameter)
        {
            if (CanExecute(parameter))
            {
                navigator
                    .Message($"Do you really want to remove exchange rate? {Environment.NewLine}{Environment.NewLine}From '{parameter.SourceCurrency}' to '{targetCurrencyUniqueCode}', rate '{parameter.Rate}', valid from '{parameter.ValidFrom}'")
                    .Button("Yes", new YesCommand(commandDispatcher, parameter, targetCurrencyUniqueCode))
                    .ButtonClose("No")
                    .Show();
            }
        }

        private class YesCommand : DomainCommand<RemoveExchangeRate>
        {
            private readonly ICommandDispatcher commandDispatcher;
            private readonly ExchangeRateModel model;
            private readonly string targetCurrencyUniqueCode;

            public YesCommand(ICommandDispatcher commandDispatcher, ExchangeRateModel model, string targetCurrencyUniqueCode)
                : base(commandDispatcher)
            {
                Ensure.NotNull(commandDispatcher, "commandDispatcher");
                Ensure.NotNull(model, "model");
                Ensure.NotNullOrEmpty(targetCurrencyUniqueCode, "targetCurrencyUniqueCode");
                this.commandDispatcher = commandDispatcher;
                this.model = model;
                this.targetCurrencyUniqueCode = targetCurrencyUniqueCode;
            }

            public override bool CanExecute() => true;

            protected override RemoveExchangeRate CreateDomainCommand() => new RemoveExchangeRate(model.SourceCurrency, targetCurrencyUniqueCode, model.ValidFrom, model.Rate);
        }
    }
}
