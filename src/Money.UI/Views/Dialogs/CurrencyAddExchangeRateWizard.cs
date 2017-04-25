using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Dialogs
{
    [NavigationParameter(typeof(CurrencyAddExchangeRateParameter))]
    public class CurrencyAddExchangeRateWizard : IWizard
    {
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;

        public async Task ShowAsync(object rawParameter)
        {
            CurrencyAddExchangeRateParameter parameter = (CurrencyAddExchangeRateParameter)rawParameter;

            CurrencyExchangeRate dialog = new CurrencyExchangeRate(queryDispatcher);
            dialog.TargetCurrency = parameter.TargetCurrency;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                await domainFacade.SetExchangeRateAsync(
                    dialog.SourceCurrency, 
                    dialog.TargetCurrency, 
                    dialog.ValidFrom, 
                    dialog.Rate
                );
            }
        }
    }
}
