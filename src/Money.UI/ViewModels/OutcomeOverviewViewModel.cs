using Money.Services.Models;
using Money.Services.Models.Queries;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Observables;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    public class OutcomeOverviewViewModel : ObservableObject
    {
        private readonly IQueryDispatcher queryDispatcher;

        /// <summary>
        /// Gets a key of the outcome.
        /// </summary>
        public IKey Key { get; private set; }

        private Price amount;

        /// <summary>
        /// Gets an amount of the outcome.
        /// </summary>
        public Price Amount
        {
            get { return amount; }
            set
            {
                if (amount != value)
                {
                    amount = value;
                    RaisePropertyChanged();

                    queryDispatcher.QueryAsync(new GetCurrencySymbol(amount.Currency)).ContinueWith(t => CurrencySymbol = t.Result);
                }
            }
        }

        private string currencySymbol;
        public string CurrencySymbol
        {
            get { return currencySymbol; }
            set
            {
                if (currencySymbol != value)
                {
                    currencySymbol = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime when;

        /// <summary>
        /// Gets a date when the outcome ocured.
        /// </summary>
        public DateTime When
        {
            get { return when; }
            set
            {
                if (when != value)
                {
                    when = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string description;

        /// <summary>
        /// Gets a description of the outcome.
        /// </summary>
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isSelected;

        /// <summary>
        /// Gets or sets if view model is selected.
        /// </summary>
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

        public OutcomeOverviewViewModel(IQueryDispatcher queryDispatcher, OutcomeOverviewModel model)
        {
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            Ensure.NotNull(model, "model");
            this.queryDispatcher = queryDispatcher;

            Key = model.Key;
            Amount = model.Amount;
            When = model.When;
            Description = model.Description;
        }
    }
}
