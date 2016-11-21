using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;
using Neptuo.Activators;

namespace Money.ViewModels
{
    /// <summary>
    /// A view model for month or year view.
    /// </summary>
    public class SummaryViewModel : ViewModel
    {
        public ObservableCollection<SummaryGroupViewModel> Groups { get; private set; }

        private SummaryGroupViewModel selectedGroup;
        public SummaryGroupViewModel SelectedGroup
        {
            get { return selectedGroup; }
            set
            {
                if (selectedGroup != value)
                {
                    selectedGroup = value;
                    RaisePropertyChanged();

                    if (selectedGroup != null)
                        selectedGroup.EnsureLoadedAsync();
                }
            }
        }

        public SummaryViewModel()
        {
            Groups = new ObservableCollection<SummaryGroupViewModel>();
        }
    }
}
