using Money.Commands;
using Money.Services;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo.Activators;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using Neptuo.Queries;
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
        private readonly ICommandDispatcher commandDispatcher = ServiceProvider.CommandDispatcher;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;

        public async Task ShowAsync(object context)
        {
            OutcomeParameter parameter = (OutcomeParameter)context;

            decimal amount = 0;
            string currency = null;
            string description = String.Empty;
            DateTime when = DateTime.Now;
            IKey categoryKey = parameter.CategoryKey;

            // Initiate categories loading...
            CategoryPicker categoryDialog = new CategoryPicker();

            OutcomeAmount amountDialog = new OutcomeAmount(queryDispatcher);
            amountDialog.PrimaryButtonText = "Next";

            if (parameter.CategoryKey.IsEmpty)
                amountDialog.SecondaryButtonText = String.Empty;
            else
                amountDialog.SecondaryButtonText = "Create today";

            if (parameter.Amount != null)
            {
                amountDialog.Value = parameter.Amount.Value;
                amountDialog.Currency = parameter.Amount.Currency;
            }

            ContentDialogResult result = await amountDialog.ShowAsync(false);
            amount = amountDialog.Value;
            currency = amountDialog.Currency;
            if (result == ContentDialogResult.None)
            {
                return;
            }
            else if (result == ContentDialogResult.Primary)
            {
                categoryDialog.PrimaryButtonText = "Next";
                categoryDialog.SecondaryButtonText = "Create today";
                if (!parameter.CategoryKey.IsEmpty)
                    categoryDialog.SelectedKey = parameter.CategoryKey;

                result = await categoryDialog.ShowAsync();
                if (result == ContentDialogResult.None)
                    return;

                categoryKey = categoryDialog.SelectedKey;
                if (result == ContentDialogResult.Primary)
                {
                    OutcomeDescription descriptionDialog = new OutcomeDescription();
                    descriptionDialog.PrimaryButtonText = "Next";
                    descriptionDialog.SecondaryButtonText = "Create today";

                    result = await descriptionDialog.ShowAsync();
                    if (result == ContentDialogResult.None && !descriptionDialog.IsEnterPressed)
                        return;

                    description = descriptionDialog.Value;

                    if (result == ContentDialogResult.Primary || descriptionDialog.IsEnterPressed)
                    {
                        OutcomeWhen whenDialog = new OutcomeWhen();
                        whenDialog.PrimaryButtonText = "Create";
                        whenDialog.SecondaryButtonText = "Cancel";
                        whenDialog.Value = when;

                        result = await whenDialog.ShowAsync();
                        if (result != ContentDialogResult.Primary)
                            return;

                        when = whenDialog.Value;
                    }
                }
            }

            await commandDispatcher.HandleAsync(new CreateOutcome(
                new Price(amount, currency),
                description,
                when,
                categoryKey
            ));
        }
    }
}
