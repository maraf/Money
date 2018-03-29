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
    public class NavigateCommand : Neptuo.Observables.Commands.Command
    {
        private readonly INavigator navigator;
        private readonly object parameter;

        private bool isExecutable;

        public bool IsExecutable
        {
            get { return isExecutable; }
            set
            {
                if (isExecutable != value)
                {
                    isExecutable = value;
                    RaiseCanExecuteChanged();
                }
            }
        }

        public NavigateCommand(INavigator navigator, object parameter)
        {
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(parameter, "parameter");
            this.navigator = navigator;
            this.parameter = parameter;

            IsExecutable = true;
        }

        public override bool CanExecute()
        {
            return IsExecutable;
        }

        public override void Execute()
        {
            navigator
                .Open(parameter)
                .Show();
        }
    }
}
