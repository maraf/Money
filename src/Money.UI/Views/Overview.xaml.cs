using Money.Services;
using Money.Services.Models;
using Money.Services.Models.Queries;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Money.Views
{
    [NavigationParameter(typeof(OverviewParameter))]
    public sealed partial class Overview : Page
    {
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private bool isDateSorted = true;
        private bool isAmountSorted;
        private bool isDescriptionSorted;

        public OverviewViewModel ViewModel
        {
            get { return (OverviewViewModel)DataContext; }
            set { DataContext = value; }
        }

        public Overview()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            OverviewParameter parameter = (OverviewParameter)e.Parameter;

            string categoryName = parameter.CategoryKey.IsEmpty 
                ? "All"
                : await queryDispatcher.QueryAsync(new GetCategoryName(parameter.CategoryKey));

            object period = null;
            IEnumerable<OutcomeOverviewModel> models = null;
            if (parameter.Month != null)
            {
                period = parameter.Month;
                models = await queryDispatcher.QueryAsync(new ListMonthOutcomeFromCategory(parameter.CategoryKey, parameter.Month));
            }

            if (parameter.Year != null)
            {
                period = parameter.Year;
                models = await queryDispatcher.QueryAsync(new ListYearOutcomeFromCategory(parameter.CategoryKey, parameter.Year));
            }

            ViewModel = new OverviewViewModel(navigator, parameter.CategoryKey, categoryName, period);
            if (models != null)
            {
                foreach (OutcomeOverviewModel model in models)
                    ViewModel.Items.Add(model);
            }
        }

        private void mfiSortDate_Click(object sender, RoutedEventArgs e)
        {
            if (isDateSorted)
            {
                ViewModel.Items.SortDescending(i => i.When);
                isDateSorted = false;
            }
            else
            {
                ViewModel.Items.Sort(i => i.When);
                isDateSorted = true;
            }

            isAmountSorted = false;
            isDescriptionSorted = false;
        }

        private void mfiSortAmount_Click(object sender, RoutedEventArgs e)
        {
            if (isAmountSorted)
            {
                ViewModel.Items.SortDescending(i => i.Amount.Value);
                isAmountSorted = false;
            }
            else
            {
                ViewModel.Items.Sort(i => i.Amount.Value);
                isAmountSorted = true;
            }

            isDateSorted = false;
            isDescriptionSorted = false;
        }

        private void mfiSortDescription_Click(object sender, RoutedEventArgs e)
        {
            if (isDescriptionSorted)
            {
                ViewModel.Items.SortDescending(i => i.Description);
                isDescriptionSorted = false;
            }
            else
            {
                ViewModel.Items.Sort(i => i.Description);
                isDescriptionSorted = true;
            }

            isAmountSorted = false;
            isDateSorted = false;
        }
    }
}
