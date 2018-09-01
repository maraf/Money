using Money.Commands;
using Money.Services;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Dialogs
{
    [NavigationParameter(typeof(CurrencyNewParameter))]
    public class CurrencyCreate : IWizard
    {
        private readonly ICommandDispatcher commandDispatcher = ServiceProvider.CommandDispatcher;
        private readonly MessageBuilder messageBuilder = ServiceProvider.MessageBuilder;

        public Task ShowAsync(object parameter) => ShowInternalAsync(new CurrencyName(), null);

        private async Task ShowInternalAsync(CurrencyName dialog, string errorMessage)
        {
            dialog.ErrorMessage = errorMessage;

            ContentDialogResult result = await dialog.ShowAsync();
            if ((result == ContentDialogResult.Primary || dialog.IsEnterPressed) && !String.IsNullOrEmpty(dialog.UniqueCode))
                await commandDispatcher.HandleAsync(new CreateCurrency(dialog.UniqueCode, dialog.Symbol));
        }
    }
}
