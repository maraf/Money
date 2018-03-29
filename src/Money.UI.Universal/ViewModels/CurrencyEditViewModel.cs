using Money.Services;
using Money.Models;
using Money.Models.Queries;
using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICommand = System.Windows.Input.ICommand;

namespace Money.ViewModels
{
    public class CurrencyEditViewModel : ObservableObject
    {
        private readonly IQueryDispatcher queryDispatcher;

        public string UniqueCode { get; private set; }

        private string symbol;
        public string Symbol
        {
            get { return symbol; }
            set
            {
                if (symbol != value)
                {
                    symbol = value;
                    RaisePropertyChanged();
                }
            }
        }

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

        public ICommand SetAsDefault => setAsDefault;
        public ICommand AddExchangeRate => addExchangeRate;
        public ICommand Delete { get; private set; }
        public ICommand DeleteExchangeRate { get; private set; }

        public CurrencyEditViewModel(INavigator navigator, ICommandDispatcher commandDispatcher, MessageBuilder messageBuilder, IQueryDispatcher queryDispatcher, string uniqueCode, string symbol)
        {
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            this.queryDispatcher = queryDispatcher;
            UniqueCode = uniqueCode;
            Symbol = symbol;
            ExchangeRates = new SortableObservableCollection<ExchangeRateModel>();

            CreateCommands(navigator, commandDispatcher, messageBuilder);
            LoadExchangeRateList();
        }

        private void CreateCommands(INavigator navigator, ICommandDispatcher commandDispatcher, MessageBuilder messageBuilder)
        {
            setAsDefault = new CurrencySetAsDefaultCommand(commandDispatcher, UniqueCode);
            addExchangeRate = new NavigateCommand(navigator, new CurrencyAddExchangeRateParameter(UniqueCode));
            Delete = new CurrencyDeleteCommand(navigator, commandDispatcher, messageBuilder, UniqueCode);
            DeleteExchangeRate = new CurrencyDeleteExchangeRateCommand(commandDispatcher, navigator, UniqueCode);
        }

        private async void LoadExchangeRateList()
        {
            if (isExchangeRateListLoaded)
                return;

            if (queryDispatcher == null)
                return;

            List<ExchangeRateModel> exchangeRates = await queryDispatcher.QueryAsync(new ListTargetCurrencyExchangeRates(UniqueCode));
            if (exchangeRates == null)
                return;

            ExchangeRates.AddRange(exchangeRates);
            ExchangeRates.SortDescending(e => e.ValidFrom);
            RaisePropertyChanged(nameof(ExchangeRates));

            isExchangeRateListLoaded = true;
        }
    }
}
