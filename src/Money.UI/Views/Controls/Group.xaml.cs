using Money.Services;
using Money.Services.Models;
using Money.Services.Models.Queries;
using Money.UI;
using Money.ViewModels;
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
        private readonly IDomainFacade domainFacade = App.Current.DomainFacade;
        private Dictionary<GroupItemViewModel, MonthModel> groupToMonth;

        public GroupViewModel ViewModel
        {
            get { return (GroupViewModel)DataContext; }
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
            new PropertyMetadata(null)
        );

        public Group()
        {
            InitializeComponent();
        }

        private async void OnPeriodChanged(DependencyPropertyChangedEventArgs e)
        {
            GroupViewModel viewModel = new GroupViewModel();
            DataContext = viewModel;
            viewModel.IsLoading = true;

            if (PeriodType == SummaryPeriodType.Month)
                await LoadMonthViewAsync(ViewModel, null);
            else if (PeriodType == SummaryPeriodType.Year)
                await LoadYearViewAsync(ViewModel, null);
            else
                throw Ensure.Exception.NotSupported(PeriodType.ToString());
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
