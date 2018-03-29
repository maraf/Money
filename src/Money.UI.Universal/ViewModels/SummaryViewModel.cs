using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.ViewModels.Navigation;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Events.Handlers;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Money.ViewModels
{
    public partial class SummaryViewModel : ViewModel, IEventHandler<OutcomeCreated>
    {
        private readonly IQueryDispatcher queryDispatcher;

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            internal set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Price totalAmount;
        public Price TotalAmount
        {
            get { return totalAmount; }
            set
            {
                if (totalAmount != value)
                {
                    totalAmount = value;
                    RaisePropertyChanged();
                }
            }
        }

        private MonthModel month;
        public MonthModel Month
        {
            get { return month; }
            set
            {
                if (month != value)
                {
                    month = value;
                    RaisePropertyChanged();

                    Year = null;
                    ReloadMonth();
                }
            }
        }

        private YearModel year;
        public YearModel Year
        {
            get { return year; }
            set
            {
                if (year != value)
                {
                    year = value;
                    RaisePropertyChanged();

                    Month = null;
                    ReloadYear();
                }
            }
        }

        public SortableObservableCollection<ISummaryItemViewModel> Items { get; private set; }

        public event Action OnItemsReloaded;

        public SummaryViewModel(INavigator navigator, IQueryDispatcher queryDispatcher)
            : base(navigator)
        {
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            this.queryDispatcher = queryDispatcher;
            Items = new SortableObservableCollection<ISummaryItemViewModel>();
        }

        private async Task ReloadMonth()
        {
            if (Month != null)
            {
                IsLoading = true;
                Items.Clear();

                TotalAmount = await queryDispatcher.QueryAsync(new GetTotalMonthOutcome(Month));
                Items.Add(new SummaryTotalViewModel(TotalAmount));

                IEnumerable<CategoryWithAmountModel> categories = await queryDispatcher.QueryAsync(new ListMonthCategoryWithOutcome(Month));
                int index = 0;
                foreach (CategoryWithAmountModel category in categories)
                {
                    Items.Insert(index, new SummaryCategoryViewModel()
                    {
                        CategoryKey = category.Key,
                        Name = category.Name,
                        Color = category.Color,
                        Icon = category.Icon,
                        Amount = category.TotalAmount
                    });
                    index++;
                }

                OnItemsReloaded?.Invoke();
                IsLoading = false;
            }
        }

        private async Task ReloadYear()
        {
            if (Year != null)
            {
                throw new NotImplementedException();
            }
        }

        public Task HandleAsync(OutcomeCreated payload)
        {
            if (Month != null)
            {
                MonthModel payloadMonth = payload.When;
                if (payloadMonth == Month)
                    return ReloadMonth();
            }
            else if (Year != null)
            {
                YearModel payloadYear = new YearModel(payload.When.Year);
                if (payloadYear == Year)
                    return ReloadYear();
            }

            return Task.CompletedTask;
        }
    }
}
