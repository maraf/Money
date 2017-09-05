using Money.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Money.Views.Converters
{
    public class SymmaryViewTypeToVisibilityConverter : IValueConverter
    {
        public SummaryViewType ViewType { get; set; }
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Visibility trueValue = Visibility.Visible;
            Visibility falseValue = Visibility.Collapsed;

            if(IsInverted)
            {
                trueValue = Visibility.Collapsed;
                falseValue = Visibility.Visible;
            }

            if (ViewType.Equals(value))
                return trueValue;

            return falseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
