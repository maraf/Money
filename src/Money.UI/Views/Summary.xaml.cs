using Money.Services.Models;
using Money.UI;
using Money.ViewModels;
using Money.ViewModels.Parameters;
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

        public SummaryViewModel ViewModel
        {
            get { return (SummaryViewModel)DataContext; }
        }

        public object SelectedPeriod
        {
            get { return GetValue(SelectedPeriodProperty); }
            set { SetValue(SelectedPeriodProperty, value); }
        }

        public static readonly DependencyProperty SelectedPeriodProperty = DependencyProperty.Register(
            "SelectedPeriod",
            typeof(object),
            typeof(Summary),
            new PropertyMetadata(null, OnSelectedPeriodChanged)
        );

        private static void OnSelectedPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Summary page = (Summary)d;
            page.OnSelectedPeriodChanged(e);
        }

        public Summary()
        {
            InitializeComponent();
            DataContext = new SummaryViewModel(ServiceProvider.QueryDispatcher);
        }

        private void OnSelectedPeriodChanged(DependencyPropertyChangedEventArgs e)
        {
            MonthModel month = SelectedPeriod as MonthModel;
            if (month != null)
            {
                ViewModel.Month = month;
                return;
            }

            YearModel year = SelectedPeriod as YearModel;
            if (year != null)
            {
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private void lvwBarGraph_ItemClick(object sender, ItemClickEventArgs e)
        {
            SummaryItemViewModel item = (SummaryItemViewModel)e.ClickedItem;
            CategoryListParameter parameter = null;

            MonthModel month = SelectedPeriod as MonthModel;
            if (month != null)
                parameter = new CategoryListParameter(item.CategoryKey, month);

            YearModel year = SelectedPeriod as YearModel;
            if (year != null)
                parameter = new CategoryListParameter(item.CategoryKey, year);

            navigator
                .Open(parameter)
                .Show();
        }
    }
}
