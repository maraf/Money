using Money.ViewModels.Navigation;
using Money.Views.Dialogs;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Navigation
{
    public class WizardNavigatorForm : INavigatorForm
    {
        private readonly Type wizardType;
        private readonly object parameter;
        private readonly Frame rootFrame;

        public WizardNavigatorForm(Type wizardType, object parameter, Frame rootFrame)
        {
            Ensure.NotNull(wizardType, "wizardType");
            Ensure.NotNull(parameter, "parameter");
            Ensure.NotNull(rootFrame, "rootFrame");
            this.wizardType = wizardType;
            this.parameter = parameter;
            this.rootFrame = rootFrame;
        }

        public async void Show()
        {
            IWizard wizard = (IWizard)Activator.CreateInstance(wizardType);
            await wizard.ShowAsync(parameter);

            bool isEmpty = rootFrame.Content == null;
            if (!isEmpty)
            {
                Template template = rootFrame.Content as Template;
                isEmpty = template != null && template.ContentFrame.Content == null;
            }

            if (isEmpty)
            {
                OutcomeCreatedGuidePost dialog = new OutcomeCreatedGuidePost();
                await dialog.ShowAsync();
            }
        }
    }
}
