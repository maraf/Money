using Money.Services;
using Money.Services.Models;
using Money.UI;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo;
using Neptuo.Queries;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Money.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [NavigationParameter(typeof(PieChartParameter))]
    public sealed partial class PieChart : Page
    {
        private readonly IDomainFacade domainFacade = App.Current.DomainFacade;
        private MonthModel month;

        public PieChart()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            month = e.Parameter as MonthModel;
            if (month != null)
            {
                SummaryViewModel viewModel = new SummaryViewModel(
                    month.ToString(),
                    domainFacade.PriceFactory,
                    new NotEmptyMonthCategoryGroupProvider(domainFacade, domainFacade.PriceFactory, month)
                );
                DataContext = viewModel;
                await viewModel.EnsureLoadedAsync();
                return;
            }

            throw Ensure.Exception.NotSupported("Unknown parameter in SummaryPage.");
        }
    }
}
