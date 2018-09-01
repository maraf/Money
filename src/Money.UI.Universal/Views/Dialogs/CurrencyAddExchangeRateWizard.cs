using Money.Commands;
using Money.Services;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo.Commands;
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
        private readonly ICommandDispatcher commandDispatcher = ServiceProvider.CommandDispatcher;
        private readonly MessageBuilder messageBuilder = ServiceProvider.MessageBuilder;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;

        public Task ShowAsync(object rawParameter)
        {
            CurrencyAddExchangeRateParameter parameter = (CurrencyAddExchangeRateParameter)rawParameter;

            CurrencyExchangeRate dialog = new CurrencyExchangeRate(queryDispatcher);
            dialog.TargetCurrency = parameter.TargetCurrency;
            return ShowInternalAsync(dialog, null);
        }

        private async Task ShowInternalAsync(CurrencyExchangeRate dialog, string errorMessage)
        {
            dialog.ErrorMessage = errorMessage;

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await commandDispatcher.HandleAsync(new SetExchangeRate(
                    dialog.SourceCurrency,
                    dialog.TargetCurrency,
                    dialog.ValidFrom,
                    dialog.Rate
                ));
            }
        }
    }
}
