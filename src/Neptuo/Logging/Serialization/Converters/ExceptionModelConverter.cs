using Neptuo.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging.Serialization.Converters
{
    /// <summary>
    /// Converter from <see cref="ExceptionModel"/> to <see cref="string"/>.
    /// </summary>
    public class ExceptionModelConverter : DefaultConverter<ExceptionModel, string>
    {
        public override bool TryConvert(ExceptionModel sourceValue, out string targetValue)
        {
            if(sourceValue == null)
            {
                targetValue = String.Empty;
            }
            else
            {
                StringBuilder result = new StringBuilder();
                result.AppendLine(sourceValue.Message);
                result.AppendLine(sourceValue.Exception.ToString());
                targetValue = result.ToString();
            }

            return true;
        }
    }
}
