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
    /// Converts between <see cref="JValue"/> and <see cref="int"/>, <see cref="long"/>, <see cref="double"/>, <see cref="float"/>, 
    /// <see cref="string"/>, <see cref="bool"/>, <see cref="decimal"/>, <see cref="DateTime"/>, <see cref="TimeSpan"/>, <see cref="Uri"/>
    /// and it's <see cref="Nullable{T}"/> counterparts.
    /// </summary>
    public class JsonPrimitiveConverter : IConverter
    {
        public static readonly Type[] Supported = new Type[] {
            typeof(int),
            typeof(long),
            typeof(double),
            typeof(float),
            typeof(string),
            typeof(bool),
            typeof(decimal),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Uri),

            typeof(int?),
            typeof(long?),
            typeof(double?),
            typeof(float?),
            typeof(bool?),
            typeof(decimal?),
            typeof(DateTime?),
            typeof(TimeSpan?),
        };

        public bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            if (sourceType == typeof(JToken) || sourceType == typeof(JValue))
                return TryConvertFromJson(targetType, (JValue)sourceValue, out targetValue);
            else if (targetType == typeof(JToken) || targetType == typeof(JValue))
                return TryConvertToJson(sourceType, sourceValue, out targetValue);

            targetValue = null;
            return false;
        }

        private bool TryConvertFromJson(Type targetType, JValue jValue, out object targetValue)
        {
            if (!Supported.Contains(targetType))
            {
                targetValue = null;
                return false;
            }

            bool isNullable = false;
            if (targetType.IsNullableType())
            {
                isNullable = true;
                targetType = targetType.GetGenericArguments()[0];
            }

            if (jValue.Value == null)
            {
                targetValue = null;
                return isNullable || targetType == typeof(string);
            }

            targetValue = null;
            if (targetType == typeof(int))
                targetValue = jValue.Value<int>();
            else if (targetType == typeof(long))
                targetValue = jValue.Value<long>();
            else if (targetType == typeof(double))
                targetValue = jValue.Value<double>();
            else if (targetType == typeof(float))
                targetValue = jValue.Value<float>();
            else if (targetType == typeof(string))
                targetValue = jValue.Value<string>();
            else if (targetType == typeof(bool))
                targetValue = jValue.Value<bool>();
            else if (targetType == typeof(decimal))
                targetValue = jValue.Value<decimal>();
            else if (targetType == typeof(DateTime))
                targetValue = jValue.Value<DateTime>();
            else if (targetType == typeof(TimeSpan))
                targetValue = jValue.Value<TimeSpan>();
            else if (targetType == typeof(Uri))
                targetValue = jValue.Value<Uri>();

            return targetValue != null;
        }

        private bool TryConvertToJson(Type sourceType, object sourceValue, out object targetValue)
        {
            targetValue = new JValue(sourceValue);
            return true;
        }
    }
}
