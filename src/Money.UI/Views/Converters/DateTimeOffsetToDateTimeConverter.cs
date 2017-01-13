using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Money.Views.Converters
{
    public class DateTimeOffsetToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new DateTimeOffset((DateTime)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            DateTimeOffset offset = (DateTimeOffset)value;
            return offset.Date;
        }
    }
}
