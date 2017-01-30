using Money.Services;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Dialogs
{
    [NavigationParameter(typeof(OutcomeParameter))]
    public class OutcomeCreate : IWizard
    {
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;

        public async Task ShowAsync(object context)
        {
            OutcomeParameter parameter = (OutcomeParameter)context;

            OutcomeAmount amountDialog = new OutcomeAmount();
            if (parameter.Amount != null)
                amountDialog.Value = (double)parameter.Amount.Value;

            ContentDialogResult result = await amountDialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
                return;

            OutcomeDescription descriptionDialog = new OutcomeDescription();

            result = await descriptionDialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
                return;

            OutcomeWhen whenDialog = new OutcomeWhen();

            result = await whenDialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
                return;

            CategoryPicker categoryDialog = new CategoryPicker();
            if (!parameter.CategoryKey.IsEmpty)
                categoryDialog.SelectedKey = parameter.CategoryKey;

            result = await categoryDialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
                return;

            await domainFacade.CreateOutcomeAsync(
                domainFacade.PriceFactory.Create((decimal)amountDialog.Value),
                descriptionDialog.Value,
                whenDialog.Value,
                categoryDialog.SelectedKey
            );

            //OutcomeCreatedGuidePost nextDialog = new OutcomeCreatedGuidePost();
            //await nextDialog.ShowAsync();
        }
    }
}
