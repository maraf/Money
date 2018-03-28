using Money.Services.Models;
using Money.ViewModels.Navigation;
using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Commands
{
    public class CurrencyDeleteExchangeRateCommand : Command<ExchangeRateModel>
    {
        private readonly IDomainFacade domainFacade;
        private readonly INavigator navigator;
        private readonly string targetCurrencyUniqueCode;

        public CurrencyDeleteExchangeRateCommand(IDomainFacade domainFacade, INavigator navigator, string targetCurrencyUniqueCode)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNullOrEmpty(targetCurrencyUniqueCode, "targetCurrencyUniqueCode");
            this.domainFacade = domainFacade;
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
                    .Button("Yes", new YesCommand(domainFacade, parameter, targetCurrencyUniqueCode))
                    .ButtonClose("No")
                    .Show();
            }
        }

        private class YesCommand : Command
        {
            private readonly IDomainFacade domainFacade;
            private readonly ExchangeRateModel model;
            private readonly string targetCurrencyUniqueCode;

            public YesCommand(IDomainFacade domainFacade, ExchangeRateModel model, string targetCurrencyUniqueCode)
            {
                Ensure.NotNull(domainFacade, "domainFacade");
                Ensure.NotNull(model, "model");
                Ensure.NotNullOrEmpty(targetCurrencyUniqueCode, "targetCurrencyUniqueCode");
                this.domainFacade = domainFacade;
                this.model = model;
                this.targetCurrencyUniqueCode = targetCurrencyUniqueCode;
            }

            public override bool CanExecute()
            {
                return true;
            }

            public override void Execute()
            {
                domainFacade.RemoveExchangeRateAsync(model.SourceCurrency, targetCurrencyUniqueCode, model.ValidFrom, model.Rate);
            }
        }
    }
}
