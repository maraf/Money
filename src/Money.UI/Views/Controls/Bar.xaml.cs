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
            new PropertyMetadata(0M)
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
            new PropertyMetadata(0M)
        );

        public Bar()
        {
            this.InitializeComponent();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            tblWidth.Text = e.NewSize.Width.ToString();
        }
    }
}
