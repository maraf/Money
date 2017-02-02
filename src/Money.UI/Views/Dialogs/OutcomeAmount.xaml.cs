using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views.Dialogs
{
    public sealed partial class OutcomeAmount : ContentDialog
    {
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(OutcomeAmount),
            new PropertyMetadata(0d)
        );

        public OutcomeAmount()
        {
            InitializeComponent();
        }

        private void OnNumberButtonClick(object sender, RoutedEventArgs e)
        {
            string value = ((Button)sender).Content as string;
            if (value != null)
                Value = Double.Parse(tbxAmount.Text + value);

            //if (tbxAmount.Text.StartsWith("0") && tbxAmount.Text.Length > 1)
            //tbxAmount.Text = tbxAmount.Text.Substring(1);
        }

        private void OnUndoClick(object sender, RoutedEventArgs e)
        {
            if (tbxAmount.Text.Length > 1)
                Value = Double.Parse(tbxAmount.Text.Substring(0, tbxAmount.Text.Length - 1));
            else
                Value = 0;
        }
    }
}
