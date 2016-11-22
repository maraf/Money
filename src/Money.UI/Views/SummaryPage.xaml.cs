using Money.ViewModels;
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
using System.ComponentModel;
using Money.UI;
using Money.Services.Models.Queries;
using Money.Services.Models;
using Neptuo;
using System.Threading.Tasks;
using Money.Services;
using Money.ViewModels.Parameters;

namespace Money.Views
{
    public sealed partial class SummaryPage : Page
    {
        public SummaryViewModel ViewModel
        {
            get { return (SummaryViewModel)DataContext; }
        }

        public SummaryPage()
        {
            InitializeComponent();
        }

        private readonly IDomainFacade domainFacade = App.Current.DomainFacade;
        private SummaryType summaryType;
        private Dictionary<SummaryGroupViewModel, MonthModel> groupToMonth;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            summaryType = e.Parameter as SummaryType? ?? SummaryType.Month;
            if (summaryType == SummaryType.Month)
                await LoadMonthViewAsync();
            else if (summaryType == SummaryType.Year)
                await LoadYearViewAsync();
            else
                throw Ensure.Exception.NotSupported(summaryType.ToString());

            //EntranceNavigationTransitionInfo.SetIsTargetElement(lvwItems, true);
        }

        private async Task LoadMonthViewAsync()
        {
            groupToMonth = new Dictionary<SummaryGroupViewModel, MonthModel>();

            SummaryViewModel viewModel = new SummaryViewModel();
            DataContext = viewModel;

            IEnumerable<MonthModel> months = await domainFacade.QueryAsync(new ListMonthWithOutcome());
            foreach (MonthModel month in months)
            {
                SummaryGroupViewModel monthViewModel = new SummaryGroupViewModel(
                    month.ToString(), 
                    domainFacade.PriceFactory,
                    new NotEmptyMonthCategoryGroupProvider(domainFacade, domainFacade.PriceFactory, month)
                );
                groupToMonth[monthViewModel] = month;
                ViewModel.Groups.Add(monthViewModel);
            }

            pvtGroups.SelectedIndex = ViewModel.Groups.Count - 1;
        }

        private async Task LoadYearViewAsync()
        {
            throw new NotImplementedException();
        }

        private void lvwItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            SummaryItemViewModel item = (SummaryItemViewModel)e.ClickedItem;
            Frame.Navigate(
                typeof(CategoryListPage), 
                new CategoryListParameter(item.CategoryKey, groupToMonth[(SummaryGroupViewModel)pvtGroups.SelectedItem]), 
                new DrillInNavigationTransitionInfo()
            );
        }

        private void pvtGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedGroup = (SummaryGroupViewModel)pvtGroups.SelectedItem;
        }
    }
}
