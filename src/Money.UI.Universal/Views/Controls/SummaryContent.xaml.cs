using Money.Events;
using Money.Models;
using Money.Models.Sorting;
using Money.ViewModels;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Neptuo;
using Neptuo.Events;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class SummaryContent : UserControl, System.IDisposable
    {
        private readonly List<UiThreadEventHandler> handlers = new List<UiThreadEventHandler>();

        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private readonly IEventHandlerCollection eventHandlers = ServiceProvider.EventHandlers;

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
            control.OnPreferedViewTypeChanged(VisualStateManager.GetVisualStateGroups(control.grdMain).First().CurrentState);
        }

        private void OnPreferedViewTypeChanged(VisualState state)
        {
            if (state.Name == "SmallSize")
            {
                switch (PreferedViewType)
                {
                    case SummaryViewType.PieChart:
                        IsPieChartPrefered = true;
                        IsBarGraphPrefered = false;
                        break;
                    case SummaryViewType.BarGraph:
                        IsPieChartPrefered = false;
                        IsBarGraphPrefered = true;
                        break;
                    default:
                        throw Ensure.Exception.NotSupported(PreferedViewType.ToString());
                }
            }
            else if (state.Name == "LargeSize")
            {
                IsPieChartPrefered = true;
                IsBarGraphPrefered = true;
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
            control.OnSortDescriptorChanged(e);
        }

        private void OnSortDescriptorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel.Items.Count != 0)
                ApplySortDescriptor();
        }

        public SummaryContent()
        {
            InitializeComponent();

            VisualStateManager
                .GetVisualStateGroups(grdMain)
                .First()
                .CurrentStateChanged += OnCurrentStateChanged;

            ViewModel = new SummaryViewModel(navigator, queryDispatcher);
            ViewModel.OnItemsReloaded += ApplySortDescriptor;

            // Bind events.
            handlers.Add(eventHandlers.AddUiThread<OutcomeCreated>(ViewModel, Dispatcher));
        }

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            OnPreferedViewTypeChanged(e.NewState);
        }

        private void ApplySortDescriptor()
        {
            if (SortDescriptor != null)
            {
                switch (SortDescriptor.Type)
                {
                    case SummarySortType.ByAmount:
                        if (SortDescriptor.Direction == SortDirection.Ascending)
                            ViewModel.Items.Sort(items => items.OfType<SummaryCategoryViewModel>().OrderBy(i => i.AmountValue));
                        else
                            ViewModel.Items.Sort(items => items.OfType<SummaryCategoryViewModel>().OrderByDescending(i => i.AmountValue));

                        break;
                    case SummarySortType.ByCategory:
                        if (SortDescriptor.Direction == SortDirection.Ascending)
                            ViewModel.Items.Sort(items => items.OfType<SummaryCategoryViewModel>().OrderBy(i => i.Name));
                        else
                            ViewModel.Items.Sort(items => items.OfType<SummaryCategoryViewModel>().OrderByDescending(i => i.Name));

                        break;
                }
            }
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
                ViewModel.Year = year;
                return;
            }

            throw Ensure.Exception.NotSupported();
        }

        private void lvwBarGraph_ItemClick(object sender, ItemClickEventArgs e)
        {
            ISummaryItemViewModel item = (ISummaryItemViewModel)e.ClickedItem;
            OpenOverview(item.CategoryKey);
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

        public void Dispose()
        {
            foreach (UiThreadEventHandler handler in handlers)
                handler.Remove(eventHandlers);
        }
    }
}
