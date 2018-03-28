using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Money.Views.Converters
{
    public class NullConverter : IValueConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return TypeConverterHelper.TryConvertValue(TrueValue, targetType);

            if (value is string)
            {
                string stringValue = value as string;
                if (stringValue == String.Empty)
                    return TypeConverterHelper.TryConvertValue(TrueValue, targetType);
            }

            return TypeConverterHelper.TryConvertValue(FalseValue, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
