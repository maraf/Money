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
        private MonthModel month;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
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

        private void lvwItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            SummaryItemViewModel item = (SummaryItemViewModel)e.ClickedItem;
            Frame.Navigate(
                typeof(CategoryListPage),
                new CategoryListParameter(item.CategoryKey, month),
                new DrillInNavigationTransitionInfo()
            );
        }
    }
}
