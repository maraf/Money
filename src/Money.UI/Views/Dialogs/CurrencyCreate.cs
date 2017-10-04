using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Dialogs
{
    [NavigationParameter(typeof(CurrencyNewParameter))]
    public class CurrencyCreate : IWizard
    {
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;

        public async Task ShowAsync(object parameter)
        {
            CurrencyName dialog = new CurrencyName();
            ContentDialogResult result = await dialog.ShowAsync();
            if ((result == ContentDialogResult.Primary || dialog.IsEnterPressed) && !String.IsNullOrEmpty(dialog.UniqueCode))
                await domainFacade.CreateCurrencyAsync(dialog.UniqueCode, dialog.Symbol);
        }
    }
}
