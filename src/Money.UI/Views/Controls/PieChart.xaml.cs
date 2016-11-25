using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            new PropertyMetadata(10d, OnThicknessChanged)
        );

        private static void OnThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PieChart pieChart = (PieChart)d;
            pieChart.Update();
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

        internal void Update()
        {
            double sum = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                PieChartItem item = (PieChartItem)ContainerFromIndex(i);
                if (item != null)
                {
                    double percentage = GetPercentage(item);
                    item.Update(sum, percentage, this);
                    sum += percentage;
                }
            }
        }

        private double GetPercentage(PieChartItem item)
        {
            double sum = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                PieChartItem currentItem = (PieChartItem)ContainerFromIndex(i);
                if (currentItem != null)
                    sum += currentItem.Value;
            }

            double value = item.Value;
            return (value / sum) * 100;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
        }
    }
}
