using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Money.Views.Converters
{
    public class CuttingConverter : IValueConverter
    {
        public int EdgeValue { get; set; }

        public object LowerValue { get; set; }
        public object EqualValue { get; set; }
        public object GreaterValue { get; set; }
        
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int intValue = (int)value;
            if (intValue > EdgeValue)
                return GreaterValue;
            else if (intValue < EdgeValue)
                return LowerValue;
            else
                return EqualValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
