using Money.Services.Models;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.Controls;
using Money.Views.Navigation;
using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using Neptuo.Queries;
using Money.Services.Models.Queries;

namespace Money.Views
{
    [NavigationParameter(typeof(SummaryParameter))]
    public sealed partial class Summary : Page, INavigatorPage
    {
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;

        private bool isAmountSorted;
        private bool isCategorySorted = true;

        public event EventHandler ContentLoaded;

        public GroupViewModel ViewModel
        {
            get { return (GroupViewModel)DataContext; }
            private set { DataContext = value; }
        }

        public Summary()
        {
            InitializeComponent();
            ViewModel = new GroupViewModel(navigator);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SummaryParameter parameter = (SummaryParameter)e.Parameter;
            ViewModel.ViewType = parameter.ViewType;

            switch (parameter.PeriodType)
            {
                case SummaryPeriodType.Month:
                    await LoadMonthViewAsync(ViewModel, parameter.Month);
                    break;
                case SummaryPeriodType.Year:
                    await LoadYearViewAsync(ViewModel, parameter.Year);
                    break;
                default:
                    throw Ensure.Exception.NotSupported(parameter.PeriodType.ToString());
            }

            ContentLoaded?.Invoke(this, EventArgs.Empty);
        }

        private async Task LoadMonthViewAsync(GroupViewModel viewModel, MonthModel prefered)
        {
            ViewModel.IsLoading = true;

            IEnumerable<MonthModel> months = await queryDispatcher.QueryAsync(new ListMonthWithOutcome());
            int? preferedIndex = null;
            int index = 0;
            foreach (MonthModel month in months)
            {
                viewModel.Add(month.ToString(), month);

                if (prefered == month)
                    preferedIndex = index;

                index++;
            }

            if (preferedIndex != null)
                pvtGroups.SelectedIndex = preferedIndex.Value;

            ViewModel.IsLoading = false;
        }

        private async Task LoadYearViewAsync(GroupViewModel viewModel, YearModel prefered)
        {
            throw new NotImplementedException();
        }

        public void DecorateParameter(SummaryParameter parameter)
        {
            if (parameter.Month == null && parameter.Year == null)
            {
                GroupItemViewModel viewModel = pvtGroups.SelectedItem as GroupItemViewModel;
                if (viewModel != null)
                {
                    MonthModel month = viewModel.Parameter as MonthModel;
                    if (month != null)
                    {
                        parameter.Month = month;
                        return;
                    }

                    YearModel year = viewModel.Parameter as YearModel;
                    if (year != null)
                    {
                        parameter.Year = year;
                        return;
                    }
                }
            }
        }

        private void mfiSortAmount_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SortDescriptor = ViewModel.SortDescriptor.Update(SummarySortType.ByAmount);
            isAmountSorted = !isAmountSorted;
            isCategorySorted = false;
        }

        private void mfiSortCategory_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SortDescriptor = ViewModel.SortDescriptor.Update(SummarySortType.ByCategory);
            isCategorySorted = !isCategorySorted;
            isAmountSorted = false;
        }
    }
}
