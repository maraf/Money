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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Money.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [NavigationParameter(typeof(CategoryOverviewParameter))]
    public sealed partial class CategoryOverview : Page
    {
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;

        public CategoryOverviewViewModel ViewModel
        {
            get { return (CategoryOverviewViewModel)DataContext; }
            set { DataContext = value; }
        }

        public CategoryOverview()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CategoryOverviewParameter parameter = (CategoryOverviewParameter)e.Parameter;

            string categoryName = await queryDispatcher.QueryAsync(new GetCategoryName(parameter.CategoryKey));
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

            ViewModel = new CategoryOverviewViewModel(navigator, parameter.CategoryKey, categoryName, period);
            if (models != null)
            {
                foreach (OutcomeOverviewModel model in models)
                    ViewModel.Items.Add(model);
            }
        }
    }
}
