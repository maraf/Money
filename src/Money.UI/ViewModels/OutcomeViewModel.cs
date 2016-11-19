using Money.Services;
using Money.ViewModels.Commands;
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
    public class OutcomeViewModel : ObservableObject
    {
        public Guid? Id { get; private set; }

        private float amount;
        public float Amount
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

        public ObservableCollection<CategoryViewModel> Categories { get; private set; }
        public ObservableCollection<Guid> SelectedCategories { get; private set; }
        public SaveOutcomeCommand Save { get; private set; }

        public OutcomeViewModel(IDomainFacade domainFacade)
        {
            SelectedCategories = new ObservableCollection<Guid>();
            Categories = new ObservableCollection<CategoryViewModel>();

            When = DateTime.Now;
            Save = new SaveOutcomeCommand(this, domainFacade);
        }

        public OutcomeViewModel(IDomainFacade domainFacade, Guid id)
            : this(domainFacade)
        {
            // TODO: Existing outcome.
            Id = id;
        }
    }
}
