using Money.Views.Navigation;
using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Money.ViewModels.Commands
{
    public class NavigateCommand : Command
    {
        private readonly INavigator navigator;
        private readonly object parameter;

        public NavigateCommand(INavigator navigator, object parameter)
        {
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(parameter, "parameter");
            this.navigator = navigator;
            this.parameter = parameter;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            navigator
                .Open(parameter)
                .Show();
        }
    }
}
