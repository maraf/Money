using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Money.ViewModels.Commands
{
    public class CreateOutcomeCommand : NavigateCommand
    {
        public CreateOutcomeCommand(INavigator navigator, OutcomeParameter parameter)
            : base(navigator, parameter)
        { }
    }
}
