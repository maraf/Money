using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Money.Views.Controls
{
    /// <summary>
    /// A single item of <see cref="PieChart"/>.
    /// </summary>
    public partial class PieChartItem : UserControl
    {
        /// <summary>
        /// Gets or sets a thickness of drawed lines.
        /// Default value is inherited from <see cref="PieChart"/>.
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
            typeof(PieChartItem),
            new PropertyMetadata(0d, OnThicknessChanged)
        );

        private static void OnThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
            new PropertyMetadata(0, OnValueChanged)
        );

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PieChartItem pc = (PieChartItem)d;
            pc.NotifyUpdate();
        }

        public object Label
        {
            get { return GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label",
            typeof(object),
            typeof(PieChartItem),
            new PropertyMetadata(null, OnLabelChanged)
        );

        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        public static readonly DependencyProperty LabelTemplateProperty = DependencyProperty.Register(
            "LabelTemplate",
            typeof(DataTemplate),
            typeof(PieChartItem),
            new PropertyMetadata(null, OnLabelChanged)
        );

        public double LabelOffset
        {
            get { return (double)GetValue(LabelOffsetProperty); }
            set { SetValue(LabelOffsetProperty, value); }
        }

        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register(
            "LabelOffset",
            typeof(double),
            typeof(PieChartItem),
            new PropertyMetadata(10d)
        );

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PieChartItem item = (PieChartItem)d;
            if (item.Label != null && item.LabelTemplate != null)
                throw new NotSupportedException("PieChartItem, set Label or LabelTemplate, not both.");

            if (item.Label != null || item.LabelTemplate != null)
                item.labelPanel.Visibility = Visibility.Visible;
            else
                item.labelPanel.Visibility = Visibility.Collapsed;

            if (item.Label != null)
                item.label.Content = item.Label;
        }

        public PieChartItem()
        {
            InitializeComponent();
            BindEvents();
            NotifyUpdate();
        }

        private void BindEvents()
        {
            label.SizeChanged += OnLabelSizeChanged;
        }

        private void OnLabelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            label.Margin = new Thickness(-(label.ActualWidth / 2), -(label.ActualHeight / 2), 0, 0);
        }

        private PieChart FindParent()
        {
            FrameworkElement parent = FindParent(this);
            if (parent == null)
                return null;

            PieChart chart = parent as PieChart;
            if (chart != null)
                return chart;

            for (int i = 0; i < 20; i++)
            {
                parent = FindParent(parent);
                chart = parent as PieChart;
                if (chart != null)
                    return chart;
            }

            return null;
        }

        private FrameworkElement FindParent(FrameworkElement element)
        {
            FrameworkElement parent = element.Parent as FrameworkElement;

            if (parent == null)
                parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

            return parent;
        }

        private void NotifyUpdate()
        {
            PieChart pieChart = FindParent();
            if (pieChart != null)
                pieChart.Update();
        }

        internal void Update(double offset, double percentage, PieChart pieChart)
        {
            double thickness = Thickness;
            if (thickness == 0)
                thickness = pieChart.Thickness;

            double minHalfSize = Math.Min(pieChart.ActualWidth, pieChart.ActualHeight) / 2;
            thickness = Math.Min(minHalfSize, thickness);

            double radius = minHalfSize - thickness / 2;
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

                double angleOffset = (offset * 360) / 100;
                double angle = (percentage * 360) / 100;

                RenderArc(radius, thickness, angleOffset, angle);

                double labelOffset = GetLabelOffset(pieChart);
                RenderLabel(radius, labelOffset, thickness, angleOffset, angle);
            }
        }

        private void RenderArc(double radius, double thickness, double angleOffset, double angle)
        {
            path.StrokeThickness = thickness;

            Point start = GetStartPoint(radius, angleOffset);
            ApplyThickness(ref start, thickness);

            Point end = GetEndPoint(radius, angleOffset, angle);
            ApplyThickness(ref end, thickness);
            NormalizePoint(ref start, ref end);

            path.Width = radius * 2 + thickness;
            path.Height = radius * 2 + thickness;

            figure.StartPoint = start;

            segment.Point = end;
            segment.Size = new Size(radius, radius);
            segment.IsLargeArc = angle > 180.0;
        }

        private void RenderLabel(double radius, double labelOffset, double thickness, double angleOffset, double angle)
        {
            radius += thickness / 2;
            Point point = CartesianCoordinate(angleOffset + angle / 2, radius + labelOffset);
            point.X += radius;
            point.Y += radius;

            label.SetValue(Canvas.LeftProperty, point.X);
            label.SetValue(Canvas.TopProperty, point.Y);
        }

        private Point GetStartPoint(double radius, double angleOffset)
        {
            Point start;
            if (angleOffset == 0)
            {
                start = new Point(radius, 0);
            }
            else
            {
                start = CartesianCoordinate(angleOffset, radius);
                start.X += radius;
                start.Y += radius;
            }

            return start;
        }

        private Point GetEndPoint(double radius, double angleOffset, double angle)
        {
            Point end = CartesianCoordinate(angleOffset + angle, radius);
            end.X += radius;
            end.Y += radius;

            return end;
        }

        private void ApplyThickness(ref Point point, double thickness)
        {
            point.X += thickness / 2;
            point.Y += thickness / 2;
        }

        private void NormalizePoint(ref Point start, ref Point end)
        {
            if (start.X == Math.Round(end.X) && start.Y == Math.Round(end.Y))
                end.X -= 0.01;
        }

        private Point CartesianCoordinate(double angle, double radius)
        {
            double angleRad = (Math.PI / 180.0) * (angle - 90);

            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);

            return new Point(x, y);
        }

        private double GetLabelOffset(PieChart pieChart)
        {
            double labelOffset = 0;
            object rawLabelOffset = ReadLocalValue(LabelOffsetProperty);
            if (rawLabelOffset == DependencyProperty.UnsetValue)
                labelOffset = pieChart.LabelOffset;
            else
                labelOffset = (double)rawLabelOffset;

            return labelOffset;
        }
    }
}
