using Money.Views;
using Neptuo.Observables.Commands;
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
    public class CreateOutcomeCommand : NavigateCommand<OutcomePage>
    {
        public CreateOutcomeCommand()
        { }

        public CreateOutcomeCommand(Guid id)
            : base(id)
        { }
    }
}
