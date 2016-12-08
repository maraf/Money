using Money.Views.Navigation;
using Neptuo;
using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    /// <summary>
    /// A view model for pages where data are grouped into months or years.
    /// </summary>
    public class GroupViewModel : ViewModel
    {
        private bool isLoading;

        /// <summary>
        /// Gets or sets whether data is currenly loading.
        /// </summary>
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets a collection of items.
        /// </summary>
        public ObservableCollection<GroupItemViewModel> Items { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public GroupViewModel(INavigator navigator)
            : base(navigator)
        {
            Items = new ObservableCollection<GroupItemViewModel>();
        }
    }
}
