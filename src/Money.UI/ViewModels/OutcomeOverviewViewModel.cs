using Money.Services.Models;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    public class OutcomeOverviewViewModel : ObservableObject
    {
        /// <summary>
        /// Gets a key of the outcome.
        /// </summary>
        public IKey Key { get; private set; }

        /// <summary>
        /// Gets an amount of the outcome.
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a date when the outcome ocured.
        /// </summary>
        public DateTime When { get; set; }

        /// <summary>
        /// Gets a description of the outcome.
        /// </summary>
        public string Description { get; private set; }

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

        public OutcomeOverviewViewModel(OutcomeOverviewModel model)
        {
            Ensure.NotNull(model, "model");
            Key = model.Key;
            Amount = model.Amount;
            When = model.When;
            Description = model.Description;
        }
    }
}
