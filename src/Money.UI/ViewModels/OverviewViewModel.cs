using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Money.Views.Navigation;
using Neptuo.Observables.Collections;
using Money.Services.Models;
using Neptuo.Models.Keys;
using Money.ViewModels.Commands;
using Money.ViewModels.Parameters;
using System.Windows.Input;
using Neptuo;

namespace Money.ViewModels
{
    /// <summary>
    /// A view model for category outcome overview.
    /// </summary>
    public class OverviewViewModel : ViewModel
    {
        /// <summary>
        /// Gets a key of the category.
        /// </summary>
        public IKey Key { get; private set; }

        /// <summary>
        /// Gets a name of the category.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a period displayed (year or month).
        /// </summary>
        public object Period { get; private set; }

        /// <summary>
        /// Gets a collection of outcome models.
        /// </summary>
        public SortableObservableCollection<OutcomeOverviewModel> Items { get; private set; }

        /// <summary>
        /// Gets a command for editing current category.
        /// </summary>
        public ICommand EditCategory { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="navigator">An instance of the navigator.</param>
        /// <param name="key">A key of the category</param>
        /// <param name="name">A name of the category.</param>
        /// <param name="period">A period displayed (year or month).</param>
        public OverviewViewModel(INavigator navigator, IKey key, string name, object period)
            : base(navigator, key)
        {
            Ensure.NotNull(key, "key");
            Key = key;
            Name = name;
            Period = period;
            Items = new SortableObservableCollection<OutcomeOverviewModel>();

            if (!key.IsEmpty)
                EditCategory = new NavigateCommand(navigator, new CategoryListParameter(key));
        }
    }
}
