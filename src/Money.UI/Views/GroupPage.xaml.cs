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

        public GroupPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            GroupViewModel viewModel = new GroupViewModel();
            DataContext = viewModel;
            viewModel.IsLoading = true;

            parameter = (GroupParameter)e.Parameter;
            if (parameter.Type == GroupType.Month)
                await LoadMonthViewAsync(viewModel);
            else if (parameter.Type == GroupType.Year)
                await LoadYearViewAsync(viewModel);
            else
                throw Ensure.Exception.NotSupported(parameter.Type.ToString());
        }

        private async Task LoadMonthViewAsync(GroupViewModel viewModel)
        {
            groupToMonth = new Dictionary<GroupItemViewModel, MonthModel>();

            IEnumerable<MonthModel> months = await domainFacade.QueryAsync(new ListMonthWithOutcome());
            foreach (MonthModel month in months)
            {
                GroupItemViewModel monthViewModel = new GroupItemViewModel(month.ToString(), month);
                groupToMonth[monthViewModel] = month;
                viewModel.Items.Add(monthViewModel);
            }

            pvtGroups.SelectedIndex = viewModel.Items.Count - 1;
        }

        private async Task LoadYearViewAsync(GroupViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        private void pvtGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
