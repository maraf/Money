using Money.Services.Models;
using Money.Services.Models.Queries;
using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Neptuo;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Queries;
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
        private readonly IQueryDispatcher queryDispatcher;

        public string UniqueCode { get; private set; }
        public string Symbol { get; private set; }

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

                    if(!isExchangeRateListLoaded)
                        LoadExchangeRateList();
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

        private bool isExchangeRateListLoaded;

        public SortableObservableCollection<ExchangeRateModel> ExchangeRates { get; private set; }

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

        public CurrencyEditViewModel(INavigator navigator, IDomainFacade domainFacade, IQueryDispatcher queryDispatcher, string uniqueCode, string symbol)
        {
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            this.queryDispatcher = queryDispatcher;
            UniqueCode = uniqueCode;
            Symbol = symbol;
            ExchangeRates = new SortableObservableCollection<ExchangeRateModel>();

            CreateCommands(navigator, domainFacade);
        }

        private void CreateCommands(INavigator navigator, IDomainFacade domainFacade)
        {
            setAsDefault = new CurrencySetAsDefaultCommand(domainFacade, UniqueCode);
            addExchangeRate = new NavigateCommand(navigator, new CurrencyAddExchangeRateParameter(UniqueCode));
        }

        private async void LoadExchangeRateList()
        {
            if (queryDispatcher == null)
                return;

            List<ExchangeRateModel> exchangeRates = await queryDispatcher.QueryAsync(new ListTargetCurrencyExchangeRates(UniqueCode));
            if (exchangeRates == null)
                return;

            ExchangeRates.AddRange(exchangeRates);
            ExchangeRates.SortDescending(e => e.ValidFrom);

            isExchangeRateListLoaded = true;
        }
    }
}
