using Money.Views.Controls;
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

namespace Money.Views.Dialogs
{
    public sealed partial class OutcomeWhen : ContentDialog
    {
        private readonly ValueChangedLock valueLock = new ValueChangedLock();

        public DateTime Value
        {
            get { return (DateTime)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", 
            typeof(DateTime), 
            typeof(OutcomeWhen), 
            new PropertyMetadata(DateTime.Now, OnValueChanged)
        );

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OutcomeWhen control = (OutcomeWhen)d;
            control.OnValueChanged();
        }

        public OutcomeWhen()
        {
            InitializeComponent();

            cavContent.MaxDate = DateTime.Today;
        }

        private void OnValueChanged()
        {
            if (valueLock.IsLocked)
                return;

            using (valueLock.Lock())
            {
                cavContent.SelectedDates.Clear();
                cavContent.SelectedDates.Add(Value);
            }
        }

        private void cavContent_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs e)
        {
            if (valueLock.IsLocked)
                return;

            using (valueLock.Lock())
            {
                if (e.AddedDates.Count > 0)
                    Value = e.AddedDates.First().Date;
                else
                    Value = DateTime.Today;
            }
        }
    }
}
