using Money.ViewModels.Navigation;
using Money.Views.Dialogs;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Money.Views.Navigation
{
    public class WizardNavigatorForm : INavigatorForm
    {
        private readonly Type wizardType;
        private readonly object parameter;
        private readonly Frame rootFrame;
        private readonly object lastParameter;

        public WizardNavigatorForm(Type wizardType, object parameter, Frame rootFrame, object lastParameter)
        {
            Ensure.NotNull(wizardType, "wizardType");
            Ensure.NotNull(parameter, "parameter");
            Ensure.NotNull(rootFrame, "rootFrame");
            this.wizardType = wizardType;
            this.parameter = parameter;
            this.rootFrame = rootFrame;
            this.lastParameter = lastParameter;
        }

        public async void Show()
        {
            IWizard wizard = (IWizard)Activator.CreateInstance(wizardType);
            await wizard.ShowAsync(parameter);

            ITemplate template = null;
            bool isEmpty = rootFrame.Content == null;
            if (!isEmpty)
            {
                template = rootFrame.Content as ITemplate;
                if (template != null)
                    isEmpty = template.ContentFrame.Content == null;
            }

            if (isEmpty)
            {
                OutcomeCreatedGuidePost dialog = new OutcomeCreatedGuidePost();
                await dialog.ShowAsync();
            }
            else if (template != null && lastParameter != null)
            {
                template.UpdateActiveMenuItem(lastParameter);
            }
        }
    }
}
