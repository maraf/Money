using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.Converters
{
    internal static class TypeConverterHelper
    {
        public static object TryConvertValue(object value, Type targetType)
        {
            if (value == null)
                return value;

            TypeConverter converter = TypeDescriptor.GetConverter(targetType);
            if (converter == null)
                return value;

            if (converter.CanConvertFrom(value.GetType()))
                return converter.ConvertFrom(value);

            return value;
        }
    }
}
