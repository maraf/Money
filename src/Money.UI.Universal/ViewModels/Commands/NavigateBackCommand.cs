using Money.ViewModels.Navigation;
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
    public class NavigateBackCommand : Neptuo.Observables.Commands.Command
    {
        private readonly INavigator navigator;

        public NavigateBackCommand(INavigator navigator)
        {
            Ensure.NotNull(navigator, "navigator");
            this.navigator = navigator;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            navigator.GoBack();
        }
    }
}
