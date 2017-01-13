using Money.Services.Models;
using Money.ViewModels;
using Neptuo;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Money.Views.Controls
{
    public sealed partial class SummaryContent : UserControl
    {
        public SummaryViewModel ViewModel
        {
            get { return (SummaryViewModel)DataContext; }
            set { DataContext = value; }
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

        public SummaryContent()
        {
            InitializeComponent();
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
    }
}
