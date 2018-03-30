using Neptuo.Converters;
using Neptuo.Formatters.Internals;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Converters
{
    /// <summary>
    /// Converts between <see cref="JValue"/> and any enum or nullable enum type.
    /// </summary>
    public class JsonEnumConverter : IConverter
    {
        private readonly JsonEnumConverterType type;

        public JsonEnumConverter(JsonEnumConverterType type)
        {
            this.type = type;
        }

        public bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            if (sourceType == typeof(JToken))
                return TryConvertFromJson(targetType, (JValue)sourceValue, out targetValue);
            else if (targetType == typeof(JToken))
                return TryConvertToJson(sourceType, sourceValue, out targetValue);

            targetValue = null;
            return false;
        }

        private bool TryConvertFromJson(Type targetType, JValue jValue, out object targetValue)
        {
            bool isNullable = false;
            if (targetType.IsNullableType())
            {
                isNullable = true;
                targetType = targetType.GetGenericArguments()[0];
            }

            if (jValue.Value == null)
            {
                targetValue = null;
                return isNullable;
            }

            if (targetType.GetTypeInfo().IsEnum)
            {
                if (type == JsonEnumConverterType.UseInderlayingValue)
                {
                    int value = jValue.Value<int>();
                    targetValue = Enum.ToObject(targetType, value);
                    return true;
                }
                else if (type == JsonEnumConverterType.UseTextName)
                {
                    string value = jValue.Value<string>();
                    return Converts.Try(typeof(string), targetType, value, out targetValue);
                }
            }

            targetValue = null;
            return false;
        }

        private bool TryConvertToJson(Type sourceType, object sourceValue, out object targetValue)
        {
            bool isNullable = false;
            if (sourceType.IsNullableType())
            {
                isNullable = true;
                sourceType = sourceType.GetGenericArguments()[0];
            }

            if (sourceValue == null)
            {
                targetValue = null;
                return isNullable;
            }

            if (sourceType.GetTypeInfo().IsEnum)
            {
                if (type == JsonEnumConverterType.UseInderlayingValue)
                {
                    targetValue = new JValue((int)sourceValue);
                    return true;
                }
                else if (type == JsonEnumConverterType.UseTextName)
                {
                    if (Converts.Try(sourceType, typeof(string), sourceValue, out targetValue))
                    {
                        targetValue = new JValue(targetValue);
                        return true;
                    }
                }

                if (isNullable)
                {
                    targetValue = null;
                    return true;
                }
            }

            targetValue = null;
            return false;
        }
    }
}
