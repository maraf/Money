using Neptuo.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Converters
{
    /// <summary>
    /// The converter from the <see cref="TimeSpan"/> to the <see cref="JToken"/> and back.
    /// </summary>
    public class TimeSpanToJsonConverter : IConverter<TimeSpan, JToken>, IConverter<JToken, TimeSpan>, IConverter<JValue, TimeSpan>
    {
        public bool TryConvert(TimeSpan sourceValue, out JToken targetValue)
        {
            targetValue = new JValue(sourceValue.ToString());
            return true;
        }

        public bool TryConvert(JToken sourceValue, out TimeSpan targetValue)
        {
            JValue source = sourceValue as JValue;
            if (source == null || source.Value == null)
            {
                targetValue = TimeSpan.Zero;
                return false;
            }

            return TimeSpan.TryParse(source.Value.ToString(), out targetValue);
        }

        public bool TryConvert(JValue sourceValue, out TimeSpan targetValue)
        {
            if (sourceValue == null || sourceValue.Value == null)
            {
                targetValue = TimeSpan.Zero;
                return false;
            }

            return TimeSpan.TryParse(sourceValue.Value.ToString(), out targetValue);
        }

        public bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            Ensure.NotNull(sourceType, "sourceType");
            Ensure.NotNull(targetType, "targetType");

            if (sourceType == typeof(TimeSpan) && targetType == typeof(JToken))
            {
                JToken target;
                if (TryConvert((TimeSpan)sourceValue, out target))
                {
                    targetValue = target;
                    return true;
                }
            }
            else if (sourceType == typeof(JToken) && targetType == typeof(TimeSpan))
            {
                TimeSpan target;
                if (TryConvert((JToken)sourceValue, out target))
                {
                    targetValue = target;
                    return true;
                }
            }
            else if (sourceType == typeof(JValue) && targetType == typeof(TimeSpan))
            {
                TimeSpan target;
                if (TryConvert((JValue)sourceValue, out target))
                {
                    targetValue = target;
                    return true;
                }
            }

            targetValue = null;
            return false;
        }
    }
}
