using Money.ViewModels.Commands;
using Neptuo;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Money.ViewModels
{
    public class CurrencyEditViewModel : ObservableObject
    {
        private readonly IDomainFacade domainFacade;

        public string Name { get; private set; }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand SetAsDefault { get; private set; }

        public CurrencyEditViewModel(IDomainFacade domainFacade, string name)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            Name = name;

            CreateCommands(domainFacade);
        }

        private void CreateCommands(IDomainFacade domainFacade)
        {
            SetAsDefault = new CurrencySetAsDefaultCommand(domainFacade, Name);
        }
    }
}
