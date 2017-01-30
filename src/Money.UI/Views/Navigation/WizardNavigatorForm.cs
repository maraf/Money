using Money.ViewModels.Navigation;
using Money.Views.Dialogs;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.Navigation
{
    public class WizardNavigatorForm : INavigatorForm
    {
        private readonly Type wizardType;
        private readonly object parameter;

        public WizardNavigatorForm(Type wizardType, object parameter)
        {
            Ensure.NotNull(wizardType, "wizardType");
            Ensure.NotNull(parameter, "parameter");
            this.wizardType = wizardType;
            this.parameter = parameter;
        }

        public void Show()
        {
            IWizard wizard = (IWizard)Activator.CreateInstance(wizardType);
            wizard.ShowAsync(parameter);
        }
    }
}
