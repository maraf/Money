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
    //[NavigationParameter(typeof(CurrencyParameter))]
    public class CurrencyCreate : IWizard
    {
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;

        public async Task ShowAsync(object parameter)
        {
            CurrencyName name = new CurrencyName();
            ContentDialogResult result = await name.ShowAsync();
            if (result == ContentDialogResult.Primary && !String.IsNullOrEmpty(name.Value))
                await domainFacade.CreateCurrencyAsync(name.Value);
        }
    }
}
