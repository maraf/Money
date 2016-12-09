using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Money.Views.Navigation;
using Neptuo.Observables.Collections;
using Money.Services.Models;

namespace Money.ViewModels
{
    /// <summary>
    /// A view model for category outcome overview.
    /// </summary>
    public class CategoryOverviewViewModel : ViewModel
    {
        /// <summary>
        /// Gets a name of the category.
        /// </summary>
        public string CategoryName { get; private set; }

        /// <summary>
        /// Gets a period displayed (year or month).
        /// </summary>
        public object Period { get; private set; }

        /// <summary>
        /// Gets a collection of outcome models.
        /// </summary>
        public ObservableCollection<OutcomeOverviewModel> Items { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="navigator">An instance of the navigator.</param>
        /// <param name="categoryName">A name of the category.</param>
        /// <param name="period">A period displayed (year or month).</param>
        public CategoryOverviewViewModel(INavigator navigator, string categoryName, object period)
            : base(navigator)
        {
            CategoryName = categoryName;
            Period = period;
            Items = new ObservableCollection<OutcomeOverviewModel>();
        }
    }
}
