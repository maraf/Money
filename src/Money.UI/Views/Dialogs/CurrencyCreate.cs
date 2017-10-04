using Money.ViewModels.Parameters;
using Money.Views.Navigation;
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
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;

        public Task ShowAsync(object parameter) => ShowInternalAsync(new CurrencyName(), null);

        private async Task ShowInternalAsync(CurrencyName dialog, string errorMessage)
        {
            dialog.ErrorMessage = errorMessage;

            ContentDialogResult result = await dialog.ShowAsync();
            if ((result == ContentDialogResult.Primary || dialog.IsEnterPressed) && !String.IsNullOrEmpty(dialog.UniqueCode))
            {
                try
                {
                    await domainFacade.CreateCurrencyAsync(dialog.UniqueCode, dialog.Symbol);
                }
                catch (CurrencyAlreadyExistsException)
                {
                    await ShowInternalAsync(dialog, String.Format("A currency with code '{0}' already exists.", dialog.UniqueCode));
                }
            }
        }
    }
}
