using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Money.Views.Converters
{
    public class BoolConverter : IValueConverter
    {
        [DefaultValue(true)]
        public bool Test { get; set; } = true;
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? boolValue = value as bool?;
            if (boolValue == null)
                boolValue = false;

            if (Test == boolValue.Value)
                return TypeConverterHelper.TryConvertValue(TrueValue, targetType);

            return TypeConverterHelper.TryConvertValue(FalseValue, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (TrueValue.Equals(value))
                return Test;
            else if (FalseValue.Equals(value))
                return !Test;

            return false;
        }
    }
}
