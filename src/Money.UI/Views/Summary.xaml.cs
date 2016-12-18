using Money.Services.Models;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.Controls;
using Money.Views.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Money.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [NavigationParameter(typeof(SummaryParameter))]
    public sealed partial class Summary : Page
    {
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private object preSelectedPeriod;

        public SummaryViewModel ViewModel
        {
            get { return (SummaryViewModel)DataContext; }
            set { DataContext = value; }
        }
        
        public bool IsPieChartPrefered
        {
            get { return (bool)GetValue(IsPieChartPreferedProperty); }
            set { SetValue(IsPieChartPreferedProperty, value); }
        }

        public static readonly DependencyProperty IsPieChartPreferedProperty = DependencyProperty.Register(
            "IsPieChartPrefered", 
            typeof(bool), 
            typeof(Summary), 
            new PropertyMetadata(false)
        );

        public bool IsBarGraphPrefered
        {
            get { return (bool)GetValue(IsBarGraphPreferedProperty); }
            set { SetValue(IsBarGraphPreferedProperty, value); }
        }

        public static readonly DependencyProperty IsBarGraphPreferedProperty = DependencyProperty.Register(
            "IsBarGraphPrefered", 
            typeof(bool), 
            typeof(Summary), 
            new PropertyMetadata(false)
        );

        public Summary()
        {
            InitializeComponent();
            ViewModel = new SummaryViewModel(ServiceProvider.Navigator, ServiceProvider.QueryDispatcher);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SummaryParameter parameter = (SummaryParameter)e.Parameter;
            switch (parameter.ViewType)
            {
                case SummaryViewType.PieChart:
                    IsPieChartPrefered = true;
                    IsBarGraphPrefered = false;
                    break;
                case SummaryViewType.BarGraph:
                    IsPieChartPrefered = false;
                    IsBarGraphPrefered = true;
                    break;
            }

            if (grpGroups.SelectedItem != null)
                OnPeriodChanged();
        }
        
        private void OnGroupSelectedItemChanged(object sender, SelectedItemEventArgs e)
        {
            if (ViewModel != null)
                OnPeriodChanged();
        }

        private void OnPeriodChanged()
        {
            MonthModel month = grpGroups.SelectedItem as MonthModel;
            if (month != null)
            {
                ViewModel.Month = month;
                return;
            }

            YearModel year = grpGroups.SelectedItem as YearModel;
            if (year != null)
            {
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private void lvwBarGraph_ItemClick(object sender, ItemClickEventArgs e)
        {
            SummaryItemViewModel item = (SummaryItemViewModel)e.ClickedItem;
            CategoryOverviewParameter parameter = null;

            MonthModel month = grpGroups.SelectedItem as MonthModel;
            if (month != null)
                parameter = new CategoryOverviewParameter(item.CategoryKey, month);

            YearModel year = grpGroups.SelectedItem as YearModel;
            if (year != null)
                parameter = new CategoryOverviewParameter(item.CategoryKey, year);

            navigator
                .Open(parameter)
                .Show();
        }

        private void mfiSortAmount_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Items.Sort(i => i.AmountValue);
        }

        private void mfiSortCategory_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Items.Sort(i => i.Name);
        }
    }
}
