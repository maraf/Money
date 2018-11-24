using Money.Events;
using Money.Models;
using Money.Models.Sorting;
using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Neptuo;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Money.ViewModels
{
    /// <summary>
    /// A view model for category outcome overview.
    /// </summary>
    public class OverviewViewModel : ViewModel,
        IEventHandler<OutcomeCreated>,
        IEventHandler<OutcomeAmountChanged>,
        IEventHandler<OutcomeDescriptionChanged>,
        IEventHandler<OutcomeWhenChanged>
    {
        private SortDescriptor<OverviewSortType> lastSortDescriptor;

        /// <summary>
        /// An event raised when a request to reload the data is made.
        /// </summary>
        public event Action Reload;

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
        public SortableObservableCollection<OutcomeOverviewViewModel> Items { get; private set; }

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
            Items = new SortableObservableCollection<OutcomeOverviewViewModel>();

            if (!key.IsEmpty)
                EditCategory = new NavigateCommand(navigator, new CategoryListParameter(key));
        }

        public void Sort(SortDescriptor<OverviewSortType> sortDescriptor)
        {
            if (sortDescriptor != null)
            {
                switch (sortDescriptor.Type)
                {
                    case OverviewSortType.ByDate:
                        if (sortDescriptor.Direction == SortDirection.Ascending)
                            Items.Sort(i => i.When);
                        else
                            Items.SortDescending(i => i.When);
                        break;

                    case OverviewSortType.ByAmount:
                        if (sortDescriptor.Direction == SortDirection.Ascending)
                            Items.Sort(i => i.Amount.Value);
                        else
                            Items.SortDescending(i => i.Amount.Value);
                        break;

                    case OverviewSortType.ByDescription:
                        if (sortDescriptor.Direction == SortDirection.Ascending)
                            Items.Sort(i => i.Description);
                        else
                            Items.SortDescending(i => i.Description);
                        break;
                }
            }

            lastSortDescriptor = sortDescriptor;
        }

        Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload)
        {
            if (Key.IsEmpty || payload.CategoryKey.Equals(Key))
                Reload?.Invoke();

            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload)
        {
            OutcomeOverviewViewModel viewModel = Items.FirstOrDefault(vm => payload.AggregateKey.Equals(vm.Key));
            if (viewModel != null)
            {
                viewModel.Amount = payload.NewValue;
                Sort(lastSortDescriptor);
            }

            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeDescriptionChanged>.HandleAsync(OutcomeDescriptionChanged payload)
        {
            OutcomeOverviewViewModel viewModel = Items.FirstOrDefault(vm => payload.AggregateKey.Equals(vm.Key));
            if (viewModel != null)
                viewModel.Description = payload.Description;

            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload)
        {
            OutcomeOverviewViewModel viewModel = Items.FirstOrDefault(vm => payload.AggregateKey.Equals(vm.Key));
            if (viewModel != null)
            {
                viewModel.When = payload.When;
                if (Period is MonthModel month)
                {
                    MonthModel newValue = payload.When;
                    if (month != newValue)
                        Items.Remove(viewModel);
                }
                else if (Period is YearModel year)
                {
                    YearModel newValue = new YearModel(payload.When.Year);
                    if (year != newValue)
                        Items.Remove(viewModel);
                }
            }

            return Task.CompletedTask;
        }
    }
}
