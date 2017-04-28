using Money.Services.Models;
using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Neptuo;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
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

        private bool isDefault;
        public bool IsDefault
        {
            get { return isDefault; }
            set
            {
                if (isDefault != value)
                {
                    isDefault = value;
                    RaisePropertyChanged();

                    setAsDefault.IsExecutable = !value;
                }
            }
        }

        private bool isStandalone;
        public bool IsStandalone
        {
            get { return isStandalone; }
            set
            {
                if (isStandalone != value)
                {
                    isStandalone = value;
                    RaisePropertyChanged();

                    addExchangeRate.IsExecutable = !value;
                }
            }
        }

        public ObservableCollection<ExchangeRateModel> ExchangeRates { get; private set; }

        private CurrencySetAsDefaultCommand setAsDefault;
        private NavigateCommand addExchangeRate;

        public ICommand SetAsDefault
        {
            get { return setAsDefault; }
        }

        public ICommand AddExchangeRate
        {
            get { return addExchangeRate; }
        }

        public CurrencyEditViewModel(INavigator navigator, IDomainFacade domainFacade, string name)
        {
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(domainFacade, "domainFacade");
            Name = name;
            ExchangeRates = new ObservableCollection<ExchangeRateModel>();

            CreateCommands(navigator, domainFacade);
        }

        private void CreateCommands(INavigator navigator, IDomainFacade domainFacade)
        {
            setAsDefault = new CurrencySetAsDefaultCommand(domainFacade, Name);
            addExchangeRate = new NavigateCommand(navigator, new CurrencyAddExchangeRateParameter(Name));
        }
    }
}
