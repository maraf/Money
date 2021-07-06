using Neptuo.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Converters
{
    /// <summary>
    /// Converts value of type <see cref="JToken"/> to any type and vice versa.
    /// </summary>
    public class JsonObjectConverter : IConverter
    {
        public bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            if (sourceType == typeof(JToken))
                return TryConvertFromJson(targetType, (JToken)sourceValue, out targetValue);
            else if (targetType == typeof(JToken))
                return TryConvertToJson(sourceType, sourceValue, out targetValue);

            targetValue = null;
            return false;
        }

        private bool TryConvertFromJson(Type targetType, JToken sourceValue, out object targetValue)
        {
            if (sourceValue.Type == JTokenType.Null)
            {
                targetValue = null;
                return true;
            }

            targetValue = JsonConvert.DeserializeObject(sourceValue.ToString(), targetType);
            return targetValue != null;
        }

        private bool TryConvertToJson(Type sourceType, object sourceValue, out object targetValue)
        {
            targetValue = JToken.FromObject(sourceValue);
            return true;
        }
    }
}
