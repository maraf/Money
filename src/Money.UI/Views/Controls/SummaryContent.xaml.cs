using Money.Services.Models;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo;
using Neptuo.Models.Keys;
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

namespace Money.Views.Controls
{
    /// <summary>
    /// User control containing month or year summary of outcomes.
    /// </summary>
    public sealed partial class SummaryContent : UserControl
    {
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;

        /// <summary>
        /// [Internal]
        /// Gets or sets a view model.
        /// </summary>
        public SummaryViewModel ViewModel
        {
            get { return (SummaryViewModel)grdMain.DataContext; }
            private set { grdMain.DataContext = value; }
        }

        /// <summary>
        /// Gets or sets a prefered view type.
        /// </summary>
        public SummaryViewType PreferedViewType
        {
            get { return (SummaryViewType)GetValue(PreferedViewTypeProperty); }
            set { SetValue(PreferedViewTypeProperty, value); }
        }

        /// <summary>
        /// A dependency property for prefered view type.
        /// </summary>
        public static readonly DependencyProperty PreferedViewTypeProperty = DependencyProperty.Register(
            "PreferedViewType",
            typeof(SummaryViewType),
            typeof(SummaryContent),
            new PropertyMetadata(SummaryViewType.BarGraph, OnPreferedViewTypeChanged)
        );

        private static void OnPreferedViewTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SummaryContent control = (SummaryContent)d;
            switch (control.PreferedViewType)
            {
                case SummaryViewType.PieChart:
                    control.IsPieChartPrefered = true;
                    control.IsBarGraphPrefered = false;
                    break;
                case SummaryViewType.BarGraph:
                    control.IsPieChartPrefered = false;
                    control.IsBarGraphPrefered = true;
                    break;
                default:
                    throw Ensure.Exception.NotSupported(control.PreferedViewType.ToString());
            }
        }

        public bool IsPieChartPrefered
        {
            get { return (bool)GetValue(IsPieChartPreferedProperty); }
            set { SetValue(IsPieChartPreferedProperty, value); }
        }

        public static readonly DependencyProperty IsPieChartPreferedProperty = DependencyProperty.Register(
            "IsPieChartPrefered",
            typeof(bool),
            typeof(SummaryContent),
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
            typeof(SummaryContent),
            new PropertyMetadata(false)
        );

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(object),
            typeof(SummaryContent),
            new PropertyMetadata(null, OnSelectedItemChanged)
        );

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SummaryContent control = (SummaryContent)d;
            control.OnSelectedItemChanged(e);
        }

        public SortDescriptor<SummarySortType> SortDescriptor
        {
            get { return (SortDescriptor<SummarySortType>)GetValue(SortDescriptorProperty); }
            set { SetValue(SortDescriptorProperty, value); }
        }

        public static readonly DependencyProperty SortDescriptorProperty = DependencyProperty.Register(
            "SortDescriptor",
            typeof(SortDescriptor<SummarySortType>),
            typeof(SummaryContent),
            new PropertyMetadata(null, OnSortDescriptorChanged)
        );

        private static void OnSortDescriptorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SummaryContent control = (SummaryContent)d;
            if (control.SortDescriptor != null)
            {
                switch (control.SortDescriptor.Type)
                {
                    case SummarySortType.ByAmount:
                        if (control.SortDescriptor.Direction == SortDirection.Ascending)
                            control.ViewModel.Items.Sort(i => i.AmountValue);
                        else
                            control.ViewModel.Items.SortDescending(i => i.AmountValue);

                        break;
                    case SummarySortType.ByCategory:
                        if (control.SortDescriptor.Direction == SortDirection.Ascending)
                            control.ViewModel.Items.Sort(i => i.Name);
                        else
                            control.ViewModel.Items.SortDescending(i => i.Name);

                        break;
                    default:
                        break;
                }
            }
        }

        public SummaryContent()
        {
            InitializeComponent();
            ViewModel = new SummaryViewModel(navigator, queryDispatcher);
        }

        private void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            MonthModel month = SelectedItem as MonthModel;
            if (month != null)
            {
                ViewModel.Month = month;
                return;
            }

            YearModel year = SelectedItem as YearModel;
            if (year != null)
            {
                throw new NotImplementedException();
            }

            throw Ensure.Exception.NotSupported();
        }

        private void lvwBarGraph_ItemClick(object sender, ItemClickEventArgs e)
        {
            SummaryItemViewModel item = (SummaryItemViewModel)e.ClickedItem;
            OpenOverview(item.CategoryKey);
        }

        private void lviSummary_Tapped(object sender, TappedRoutedEventArgs e)
        {
            OpenOverview(KeyFactory.Empty(typeof(Category)));
        }

        private void OpenOverview(IKey categoryKey)
        {
            OverviewParameter parameter = null;

            MonthModel month = SelectedItem as MonthModel;
            if (month != null)
                parameter = new OverviewParameter(categoryKey, month);

            YearModel year = SelectedItem as YearModel;
            if (year != null)
                parameter = new OverviewParameter(categoryKey, year);

            navigator
                .Open(parameter)
                .Show();
        }
    }
}
