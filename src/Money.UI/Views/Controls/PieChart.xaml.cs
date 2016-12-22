using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Money.Views.Controls
{
    /// <summary>
    /// A PieChart-like items control.
    /// </summary>
    public partial class PieChart : ItemsControl
    {
        /// <summary>
        /// Gets or sets a thickness of drawed lines.
        /// Default value is <c>10</c>.
        /// </summary>
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        /// <summary>
        /// A dependency property for getting or setting a thickenss of drawer lines.
        /// </summary>
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness",
            typeof(double),
            typeof(PieChart),
            new PropertyMetadata(Double.PositiveInfinity, OnThicknessChanged)
        );

        private static void OnThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PieChart pieChart = (PieChart)d;
            pieChart.Update();
        }

        public string ValuePath
        {
            get { return (string)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        public static readonly DependencyProperty ValuePathProperty = DependencyProperty.Register(
            "ValuePath",
            typeof(string),
            typeof(PieChart),
            new PropertyMetadata(null)
        );

        public string ForegroundPath
        {
            get { return (string)GetValue(ForegroundPathProperty); }
            set { SetValue(ForegroundPathProperty, value); }
        }

        public static readonly DependencyProperty ForegroundPathProperty = DependencyProperty.Register(
            "ForegroundPath",
            typeof(string),
            typeof(PieChart),
            new PropertyMetadata(null)
        );

        public string LabelPath
        {
            get { return (string)GetValue(LabelPathProperty); }
            set { SetValue(LabelPathProperty, value); }
        }

        public static readonly DependencyProperty LabelPathProperty = DependencyProperty.Register(
            "LabelPath",
            typeof(string),
            typeof(PieChart),
            new PropertyMetadata(null)
        );

        public double LabelOffset
        {
            get { return (double)GetValue(LabelOffsetProperty); }
            set { SetValue(LabelOffsetProperty, value); }
        }

        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register(
            "LabelOffset",
            typeof(double),
            typeof(PieChart),
            new PropertyMetadata(10d, OnLabelOffsetChanged)
        );

        private static void OnLabelOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PieChart chart = (PieChart)d;
            chart.Update();
        }

        public PieChart()
        {
            InitializeComponent();
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            Update();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            PieChartItem item = new PieChartItem();

            if (!String.IsNullOrEmpty(ValuePath))
                item.SetBinding(PieChartItem.ValueProperty, new Binding() { Path = new PropertyPath(ValuePath) });

            if (!String.IsNullOrEmpty(ForegroundPath))
                item.SetBinding(PieChartItem.ForegroundProperty, new Binding() { Path = new PropertyPath(ForegroundPath) });

            if (!String.IsNullOrEmpty(LabelPath))
                item.SetBinding(PieChartItem.LabelProperty, new Binding() { Path = new PropertyPath(LabelPath) });

            return item;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PieChartItem;
        }
        
        protected override void PrepareContainerForItemOverride(DependencyObject element, object dataItem)
        {
            PieChartItem item = element as PieChartItem;
            if (item != null)
                item.LabelTemplate = ItemTemplate;
        }

        internal void Update()
        {
            double sum = 0;
            foreach (PieChartItem item in EnumerateItems())
            {
                double percentage = GetPercentage(item);
                item.Update(sum, percentage, this);
                sum += percentage;
            }
        }

        private double GetPercentage(PieChartItem item)
        {
            double sum = EnumerateItems().Sum(i => i.Value);
            double value = item.Value;

            return (value / sum) * 100;
        }

        private IEnumerable<PieChartItem> EnumerateItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                PieChartItem item = Items[i] as PieChartItem;
                if (item == null)
                    item = (PieChartItem)ContainerFromIndex(i);

                if (item != null)
                    yield return item;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            Update();
            return finalSize;
        }
    }
}
