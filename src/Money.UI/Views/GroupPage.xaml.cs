using Money.Services;
using Money.Services.Models;
using Money.Services.Models.Queries;
using Money.UI;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.DesignData;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Money.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [NavigationParameter(typeof(GroupParameter))]
    public sealed partial class GroupPage : Page, IParameterDecorator
    {
        private readonly IDomainFacade domainFacade = App.Current.DomainFacade;
        private GroupParameter parameter;
        private Dictionary<GroupItemViewModel, MonthModel> groupToMonth;

        public GroupViewModel ViewModel
        {
            get { return (GroupViewModel)DataContext; }
        }

        public GroupPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            parameter = (GroupParameter)e.Parameter;
            Navigate(parameter);
        }

        public async void Navigate(GroupParameter parameter)
        {
            GroupViewModel viewModel = new GroupViewModel();
            DataContext = viewModel;
            viewModel.IsLoading = true;

            IGroupParameter groupParameter = parameter.Inner as IGroupParameter;

            if (parameter.Type == GroupType.Month)
                await LoadMonthViewAsync(ViewModel, groupParameter?.Month);
            else if (parameter.Type == GroupType.Year)
                await LoadYearViewAsync(ViewModel, groupParameter?.Year);
            else
                throw Ensure.Exception.NotSupported(parameter.Type.ToString());
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
            // TODO: Select page based on parameter.

            //PivotItem item = (PivotItem)pvtGroups.ContainerFromIndex(pvtGroups.SelectedIndex);
            //if (item != null)
            //{
            //    ContentPresenter presenter = (ContentPresenter)item.FindName("copContent");
            //    if (presenter != null)
            //        presenter.Content = new OutcomePage();
            //}

            GroupItemViewModel viewModel = (GroupItemViewModel)pvtGroups.SelectedItem;

            if (pvtGroups.SelectedIndex == 0)
                frmContent.Navigate(typeof(CategoryListPage));
            else if(pvtGroups.SelectedIndex == 3)
                frmContent.Navigate(typeof(PieChart), viewModel.Parameter);
            else
                frmContent.Navigate(typeof(SummaryPage), viewModel.Parameter);

            //((Frame)Window.Current.Content).BackStack.Add(frmContent.BackStack.Last());
        }

        public object Decorate(object parameter)
        {
            IGroupParameter groupParameter = parameter as IGroupParameter;
            if (groupParameter != null)
            {
                GroupItemViewModel viewModel = (GroupItemViewModel)pvtGroups.SelectedItem;
                if (this.parameter.Type == GroupType.Month)
                    groupParameter.Month = (MonthModel)viewModel.Parameter;
                else if (this.parameter.Type == GroupType.Year)
                    groupParameter.Year = (YearModel)viewModel.Parameter;
                else
                    throw Ensure.Exception.NotSupported(this.parameter.Type.ToString());
            }

            return parameter;
        }
    }
}
