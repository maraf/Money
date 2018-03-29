using Money.Services;
using Money.Models;
using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Neptuo.Commands;

namespace Money.ViewModels
{
    public class OutcomeViewModel : ObservableObject
    {
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

        public ObservableCollection<CategoryModel> Categories { get; private set; }
        public ObservableCollection<IKey> SelectedCategories { get; private set; }
        public SaveOutcomeCommand Save { get; private set; }

        public OutcomeViewModel(INavigator navigator, ICommandDispatcher commandDispatcher)
        {
            SelectedCategories = new ObservableCollection<IKey>();
            Categories = new ObservableCollection<CategoryModel>();

            When = DateTime.Now;
            Save = new SaveOutcomeCommand(navigator, this, commandDispatcher);
        }
    }
}
