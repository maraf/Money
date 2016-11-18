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

namespace Money.Views
{
    public sealed partial class SummaryPage : Page
    {
        public SummaryViewModel ViewModel
        {
            get { return (SummaryViewModel)DataContext; }
        }

        public Price TotalAmount
        {
            get { return (Price)GetValue(TotalAmountProperty); }
            set { SetValue(TotalAmountProperty, value); }
        }

        public static readonly DependencyProperty TotalAmountProperty = DependencyProperty.Register(
            "TotalAmount", 
            typeof(decimal), 
            typeof(SummaryPage), 
            new PropertyMetadata(Price.Zero("CZK"))
        );

        public SummaryPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SummaryViewModel viewModel = new DesignData.ViewModelLocator().Summary;
            DataContext = viewModel;
            TotalAmount = viewModel.TotalAmount;

            viewModel.PropertyChanged += OnViewModelPropertyChanged;

            EntranceNavigationTransitionInfo.SetIsTargetElement(lvwItems, true);
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SummaryViewModel.TotalAmount))
                TotalAmount = ViewModel.TotalAmount;
        }

        private void lvwItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            SummaryItemViewModel item = (SummaryItemViewModel)e.ClickedItem;
            Frame.Navigate(typeof(ListPage), item.GroupId, new DrillInNavigationTransitionInfo());
        }
    }
}
