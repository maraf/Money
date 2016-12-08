using Money.Services;
using Money.Services.Models;
using Money.Services.Models.Queries;
using Money.ViewModels;
using Money.Views.Navigation;
using Neptuo;
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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Money.Views.Controls
{
    public sealed partial class Group : UserControl
    {
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;
        private Dictionary<GroupItemViewModel, MonthModel> groupToMonth;

        public GroupViewModel ViewModel
        {
            get { return (GroupViewModel)DataContext; }
            private set { DataContext = value; }
        }

        public SummaryPeriodType PeriodType
        {
            get { return (SummaryPeriodType)GetValue(PeriodTypeProperty); }
            set { SetValue(PeriodTypeProperty, value); }
        }

        public static readonly DependencyProperty PeriodTypeProperty = DependencyProperty.Register(
            "PeriodType",
            typeof(SummaryPeriodType),
            typeof(Group),
            new PropertyMetadata(SummaryPeriodType.Month, OnPeriodChanged)
        );

        private static void OnPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Group control = (Group)d;
            control.OnPeriodChanged(e);
        }

        public object SelectedItem
        {
            get { return (MonthModel)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(object),
            typeof(Group),
            new PropertyMetadata(null, OnSelectedItemChanged)
        );

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Group control = (Group)d;
            control.OnSelectedItemChanged(e);
        }

        public Group()
        {
            InitializeComponent();
        }

        private ValueChangedLock change = new ValueChangedLock();

        private async void OnPeriodChanged(DependencyPropertyChangedEventArgs e)
        {
            if (change.IsLocked)
                return;

            using (change.Lock())
            {
                if (ViewModel == null)
                    ViewModel = new GroupViewModel(navigator);

                ViewModel.IsLoading = true;
                if (PeriodType == SummaryPeriodType.Month)
                    await LoadMonthViewAsync(ViewModel, SelectedItem as MonthModel);
                else if (PeriodType == SummaryPeriodType.Year)
                    await LoadYearViewAsync(ViewModel, SelectedItem as YearModel);
                else
                    throw Ensure.Exception.NotSupported(PeriodType.ToString());

                ViewModel.IsLoading = false;
            }
        }

        private void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (change.IsLocked)
                return;

            MonthModel month = SelectedItem as MonthModel;
            if (month != null)
            {
                GroupItemViewModel viewModel = ViewModel.Items.FirstOrDefault(i => (i.Parameter as MonthModel) == month);
                if (viewModel != null)
                    pvtGroups.SelectedItem = viewModel;
            }
        }

        private async Task LoadMonthViewAsync(GroupViewModel viewModel, MonthModel prefered)
        {
            groupToMonth = new Dictionary<GroupItemViewModel, MonthModel>();

            IEnumerable<MonthModel> months = await domainFacade.QueryAsync(new ListMonthWithOutcome());
            int? preferedIndex = null;
            int index = 0;
            foreach (MonthModel month in months)
            {
                GroupItemViewModel monthViewModel = new GroupItemViewModel(month.ToString(), month);
                groupToMonth[monthViewModel] = month;

                if (prefered == month)
                    preferedIndex = index;

                viewModel.Items.Add(monthViewModel);
                index++;
            }

            if (preferedIndex == null)
                preferedIndex = viewModel.Items.Count - 1;

            pvtGroups.SelectedIndex = preferedIndex.Value;
        }

        private async Task LoadYearViewAsync(GroupViewModel viewModel, YearModel prefered)
        {
            throw new NotImplementedException();
        }

        private void pvtGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupItemViewModel viewModel = (GroupItemViewModel)pvtGroups.SelectedItem;
            SelectedItem = viewModel.Parameter;
        }
    }
}
