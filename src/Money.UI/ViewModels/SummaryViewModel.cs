using Money.Services.Models;
using Money.Services.Models.Queries;
using Money.Views.Navigation;
using Neptuo;
using Neptuo.Activators;
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
    public partial class SummaryViewModel : ViewModel
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

        public SortableObservableCollection<SummaryItemViewModel> Items { get; private set; }

        public SummaryViewModel(INavigator navigator, IQueryDispatcher queryDispatcher)
            : base(navigator)
        {
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            this.queryDispatcher = queryDispatcher;
            Items = new SortableObservableCollection<SummaryItemViewModel>();
        }
        
        private async Task ReloadMonth()
        {
            if (Month != null)
            {
                IsLoading = true;
                Items.Clear();

                IEnumerable<CategoryWithAmountModel> categories = await queryDispatcher.QueryAsync(new ListMonthCategoryWithOutcome(Month));
                foreach (CategoryWithAmountModel category in categories)
                {
                    Items.Add(new SummaryItemViewModel()
                    {
                        CategoryKey = category.Key,
                        Name = category.Name,
                        Color = category.Color,
                        Amount = category.TotalAmount
                    });
                }
                
                TotalAmount = await queryDispatcher.QueryAsync(new GetTotalMonthOutcome(Month));
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
    }
}
