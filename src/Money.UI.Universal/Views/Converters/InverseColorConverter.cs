using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace Money.Views.Converters
{
    public class InverseColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color color = (Color)value;

            int grayScale = (color.R + color.G + color.B) / 3;
            return grayScale > (Byte.MaxValue / 2)
                ? Colors.Black
                : Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
