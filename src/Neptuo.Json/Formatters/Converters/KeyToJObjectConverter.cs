using Neptuo.Converters;
using Neptuo.Models.Keys;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Converters
{
    /// <summary>
    /// The base class for converting keys to and from <see cref="JObject"/>.
    /// </summary>
    /// <typeparam name="T">The type of the key.</typeparam>
    public abstract class KeyToJObjectConverter<T>
        where T : IKey
    {
        public bool TryConvert(T sourceValue, out JToken targetValue)
        {
            if (sourceValue == null)
            {
                targetValue = null;
                return false;
            }

            JObject target;
            if (TryConvert(sourceValue, out target))
            {
                targetValue = target;
                return true;
            }

            targetValue = null;
            return false;
        }

        protected abstract bool TryConvert(T source, out JObject target);

        public bool TryConvert(JToken sourceValue, out T targetValue)
        {
            JObject source = sourceValue as JObject;
            if (source == null)
            {
                targetValue = default(T);
                return false;
            }

            return TryConvert(source, out targetValue);
        }

        protected abstract bool TryConvert(JObject source, out T target);

        public bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            Ensure.NotNull(sourceType, "sourceType");
            Ensure.NotNull(targetType, "targetType");

            if (sourceType == typeof(T) && targetType == typeof(JToken))
            {
                JToken target;
                if (TryConvert((T)sourceValue, out target))
                {
                    targetValue = target;
                    return true;
                }
            }
            else if (sourceType == typeof(JToken) && targetType == typeof(T))
            {
                T target;
                if (TryConvert((JToken)sourceValue, out target))
                {
                    targetValue = target;
                    return true;
                }
            }
            else if (sourceType == typeof(List<T>) && targetType == typeof(JToken))
            {
                JArray result = new JArray();
                IEnumerable<T> enumerable = (IEnumerable<T>)sourceValue;
                foreach (T item in enumerable)
                {
                    JToken target;
                    if (TryConvert(item, out target))
                        result.Add(target);
                }

                targetValue = result;
                return true;
            }
            else if (sourceType == typeof(JToken) && targetType == typeof(IEnumerable<T>))
            {
                List<T> result = new List<T>();
                JArray array = sourceValue as JArray;
                if (array != null)
                {
                    foreach (JToken item in array)
                    {
                        T target;
                        if (TryConvert(item, out target))
                            result.Add(target);
                    }

                    targetValue = result;
                    return true;
                }
            }

            targetValue = null;
            return false;
        }
    }
}
