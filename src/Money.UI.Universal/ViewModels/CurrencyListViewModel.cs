using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Neptuo;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Money.ViewModels
{
    public class CurrencyListViewModel : ObservableObject
    {
        public ObservableCollection<CurrencyEditViewModel> Items { get; private set; }
        public ICommand New { get; private set; }

        public CurrencyListViewModel(INavigator navigator)
        {
            Items = new ObservableCollection<CurrencyEditViewModel>();
            New = new NavigateCommand(navigator, new CurrencyNewParameter());
        }
    }
}
