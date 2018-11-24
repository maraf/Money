using Money.Models.Sorting;
using Money.ViewModels.Navigation;
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

        private SummaryViewType viewType;

        /// <summary>
        /// Gets or sets a prefered view type.
        /// </summary>
        public SummaryViewType ViewType
        {
            get { return viewType; }
            set
            {
                if (viewType != value)
                {
                    viewType = value;
                    RaisePropertyChanged();
                }
            }
        }

        private SortDescriptor<SummarySortType> sortDescriptor;

        /// <summary>
        /// Gets or sets a sort descriptor.
        /// </summary>
        public SortDescriptor<SummarySortType> SortDescriptor
        {
            get { return sortDescriptor; }
            set
            {
                if (sortDescriptor != value)
                {
                    sortDescriptor = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ObservableCollection<GroupItemViewModel> items;

        /// <summary>
        /// Gets a collection of items.
        /// </summary>
        public IEnumerable<GroupItemViewModel> Items
        {
            get { return items; }
        }
        
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public GroupViewModel(INavigator navigator)
            : base(navigator)
        {
            items = new ObservableCollection<GroupItemViewModel>();
        }

        /// <summary>
        /// Adds an item to <see cref="Items"/>.
        /// </summary>
        /// <param name="title">A title of the group.</param>
        /// <param name="parameter">A navigation parameter of the group.</param>
        /// <returns>A newly created view model.</returns>
        public GroupItemViewModel Add(string title, object parameter)
        {
            GroupItemViewModel item = new GroupItemViewModel(this, title, parameter);
            items.Add(item);
            return item;
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}
