using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class Bar : UserControl
    {
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", 
            typeof(decimal), 
            typeof(Bar), 
            new PropertyMetadata(0M, OnPropertyChanged)
        );

        public decimal Max
        {
            get { return (decimal)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
            "Max", 
            typeof(decimal), 
            typeof(Bar), 
            new PropertyMetadata(0M, OnPropertyChanged)
        );

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill", 
            typeof(Brush), 
            typeof(Bar), 
            new PropertyMetadata(new SolidColorBrush(Colors.Black))
        );
        
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Bar bar = (Bar)d;

            if (bar.Value > 0)
            {
                double ratio = (double)bar.Value / (double)bar.Max;
                bar.rctContent.Width = ratio * bar.ActualWidth;
            }
        }

        public Bar()
        {
            this.InitializeComponent();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //tblWidth.Text = e.NewSize.Width.ToString();
        }
    }
}
