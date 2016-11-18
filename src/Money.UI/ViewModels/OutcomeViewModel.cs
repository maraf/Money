using Money.ViewModels.Commands;
using Money.ViewModels.Parameters;
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
    public class OutcomeViewModel : ObservableObject
    {
        public Guid? Id { get; private set; }

        private decimal amount;
        public decimal Amount
        {
            get { return amount; }
            set
            {
                if (amount != value)
                {
                    amount = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string description;
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

        private DateTime when;
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

        public SaveOutcomeCommand Save { get; private set; }

        public OutcomeViewModel()
        {
            When = DateTime.Now;
            Save = new SaveOutcomeCommand(this);
        }

        public OutcomeViewModel(Guid id)
            : this()
        {
            // TODO: Existing outcome.
            Id = id;
        }

        public OutcomeViewModel(OutcomeDefaultsModel defaults)
            : this()
        {
            Ensure.NotNull(defaults, "defaults");

            if (defaults.Amount != null)
                Amount = defaults.Amount.Value;

            if (defaults.Description != null)
                Description = defaults.Description;
        }
    }
}
