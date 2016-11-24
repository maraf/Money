using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PieChartTest
{
    /// <summary>
    /// Interaction logic for PieChart.xaml
    /// </summary>
    public partial class PieChart : ItemsControl
    {
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

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

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            Update();
        }

        internal void Update()
        {
            double sum = 0;
            foreach (PieChartItem item in Items.SourceCollection)
            {
                double percentage = GetPercentage(item);
                item.Update(sum, percentage, this);
                sum += percentage;
            }
        }

        private double GetPercentage(PieChartItem item)
        {
            double sum = Items.SourceCollection
                .OfType<PieChartItem>()
                .ToList()
                .Sum(i => i.Value);

            double value = item.Value;
            return (value / sum) * 100;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
        }
    }
}
