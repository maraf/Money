using Money.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Money.Views.Converters
{
    public class ExchangeRateListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IReadOnlyList<ExchangeRateModel> models = (IReadOnlyList<ExchangeRateModel>)value;
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < models.Count; i++)
            {
                if (result.Length > 0)
                {
                    if (i == models.Count - 1)
                        result.Append(" and ");
                    else
                        result.Append(", ");
                }

                result.Append(models[i].SourceCurrency);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
