using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PieChartTest
{
    /// <summary>
    /// Interaction logic for PieChart.xaml
    /// </summary>
    public partial class PieChartItem : UserControl
    {
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness",
            typeof(double),
            typeof(PieChartItem),
            new PropertyMetadata(OnThicknessChanged)
        );

        private static void OnThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PieChartItem pc = (PieChartItem)d;
            pc.NotifyUpdate();
        }

        private static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PieChartItem pc = (PieChartItem)d;
            pc.NotifyUpdate();
        }
        
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(int),
            typeof(PieChartItem),
            new PropertyMetadata(10, OnValueChanged)
        );

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PieChartItem pc = (PieChartItem)d;
            pc.NotifyUpdate();
        }

        public double StartAngle { get; set; }
        public double Angle { get; set; }

        public PieChartItem()
        {
            InitializeComponent();
            NotifyUpdate();
        }

        private void NotifyUpdate()
        {
            PieChart pieChart = (PieChart)Parent;
            if (pieChart != null)
                pieChart.Update();
        }

        internal void Update(double offset, double percentage, PieChart pieChart)
        {
            double thickness = Thickness;
            if (thickness == 0)
                thickness = pieChart.Thickness;

            double radius = Math.Min(grdMain.ActualWidth, grdMain.ActualHeight) / 2 - thickness / 2;
            if (radius > 0)
            {
                if (offset < 0)
                    offset = 0;
                else if (offset >= 100)
                    offset = 99.9999d;

                if (percentage < 0)
                    percentage = 0;
                if (percentage >= 100)
                    percentage = 99.9999d;

                StartAngle = (offset * 360) / 100;
                Angle = (percentage * 360) / 100;

                RenderArc(radius, thickness);
            }
        }

        public void RenderArc(double radius, double thickness)
        {
            pathRoot.StrokeThickness = thickness;

            Point startPoint;
            if (StartAngle == 0)
            {
                startPoint = new Point(radius, 0);
            }
            else
            {
                startPoint = ComputeCartesianCoordinate(StartAngle, radius);
                startPoint.X += radius;
                startPoint.Y += radius;
            }

            Point endPoint = ComputeCartesianCoordinate(StartAngle + Angle, radius);
            endPoint.X += radius;
            endPoint.Y += radius;

            startPoint.X += thickness / 2;
            startPoint.Y += thickness / 2;
            endPoint.X += thickness / 2;
            endPoint.Y += thickness / 2;

            pathRoot.Width = radius * 2 + thickness;
            pathRoot.Height = radius * 2 + thickness;

            bool largeArc = Angle > 180.0;

            Size outerArcSize = new Size(radius, radius);

            pathFigure.StartPoint = startPoint;

            if (startPoint.X == Math.Round(endPoint.X) && startPoint.Y == Math.Round(endPoint.Y))
                endPoint.X -= 0.01;

            arcSegment.Point = endPoint;
            arcSegment.Size = outerArcSize;
            arcSegment.IsLargeArc = largeArc;
        }

        private Point ComputeCartesianCoordinate(double angle, double radius)
        {
            double angleRad = (Math.PI / 180.0) * (angle - 90);

            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);

            return new Point(x, y);
        }
    }
}
