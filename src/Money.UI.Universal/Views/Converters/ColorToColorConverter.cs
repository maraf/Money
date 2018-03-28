using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Money.Views.Converters
{
    public class ColorToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Color color1)
                return ColorConverter.Map(color1);
            else if (value is Windows.UI.Color color2)
                return ColorConverter.Map(color2);

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) 
            => Convert(value, targetType, parameter, language);
    }
}
