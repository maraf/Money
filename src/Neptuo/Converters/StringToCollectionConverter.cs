using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// Converter for converting string value (splitted using defined separator) to one of supported collection types.
    /// </summary>
    /// <typeparam name="TTargetItem">Target collection item value.</typeparam>
    public class StringToCollectionConverter<TTargetItem> :
        IConverter<string, TTargetItem>, 
        IConverter<string, List<TTargetItem>>,
        IConverter<string, IList<TTargetItem>>, 
        IConverter<string, ICollection<TTargetItem>>,
        IConverter<string, IEnumerable<TTargetItem>>
    {
        private readonly string separator;
        private readonly IConverter<string, TTargetItem> itemConverter;

        /// <summary>
        /// Creates new instance with item <paramref name="separator"/> and inner <paramref name="itemConverter"/>.
        /// </summary>
        /// <param name="separator">Item separator.</param>
        /// <param name="itemConverter">Converter for single item.</param>
        public StringToCollectionConverter(string separator, IConverter<string, TTargetItem> itemConverter)
        {
            Ensure.NotNullOrEmpty(separator, "separator");
            Ensure.NotNull(itemConverter, "itemConverter");
            this.separator = separator;
            this.itemConverter = itemConverter;
        }

        /// <summary>
        /// Splits <paramref name="sourceValue"/> to enumeration of items.
        /// </summary>
        /// <param name="sourceValue">Source value.</param>
        /// <returns><paramref name="sourceValue"/> splitted be the defined separator.</returns>
        protected IEnumerable<string> SplitSourceValue(string sourceValue)
        {
            if (String.IsNullOrEmpty(sourceValue))
                return Enumerable.Empty<string>();

            return sourceValue.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/> to list of items of <typeparamref name="TTargetItem"/>.
        /// </summary>
        /// <param name="sourceValue">Source value.</param>
        /// <param name="targetValue">Target value.</param>
        /// <returns><c>true</c> if <paramref name="sourceValue"/> can be converted to list of items of <typeparamref name="TTargetItem"/>.</returns>
        protected bool TryConvertList(string sourceValue, out List<TTargetItem> targetValue)
        {
            bool hasError = false;
            List<TTargetItem> result = new List<TTargetItem>();
            IEnumerable<string> sourceValues = SplitSourceValue(sourceValue);
            foreach (string itemValue in sourceValues)
            {
                TTargetItem item;
                if (itemConverter.TryConvert(itemValue, out item))
                {
                    result.Add(item);
                }
                else
                {
                    hasError = true;
                    break;
                }
            }

            if (hasError)
                result = null;

            targetValue = result;
            return !hasError;
        }

        bool IConverter.TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            if(sourceType != typeof(string))
            {
                targetValue = null;
                return false;
            }

            // Not generic typ return as list.
            if (sourceType != typeof(IEnumerable))
            {
                List<TTargetItem> result;
                bool success = TryConvertList((string)sourceValue, out result);
                targetValue = result;
                return success;
            }

            // If not generic or has more than one generic parameter or item can't be of type TItemTarget.
            if (!targetType.GetTypeInfo().IsGenericType || targetType.GenericTypeArguments.Length != 1 || targetType.IsAssignableFrom(targetType.GenericTypeArguments[0]))
            {
                targetValue= null;
                return false;
            }

            Type genericType = targetType.GetGenericTypeDefinition();
            if (genericType.IsAssignableFrom(typeof(List<>)))
            {
                List<TTargetItem> result;
                bool success = TryConvertList((string)sourceValue, out result);
                targetValue = result;
                return success;
            }

            targetValue = null;
            return false;
        }

        bool IConverter<string, TTargetItem>.TryConvert(string sourceValue, out TTargetItem targetValue)
        {
            return itemConverter.TryConvert(sourceValue, out targetValue);
        }

        bool IConverter<string, List<TTargetItem>>.TryConvert(string sourceValue, out List<TTargetItem> targetValue)
        {
            List<TTargetItem> result;
            bool success = TryConvertList(sourceValue, out result);

            targetValue = result;
            return success;
        }

        bool IConverter<string, IList<TTargetItem>>.TryConvert(string sourceValue, out IList<TTargetItem> targetValue)
        {
            List<TTargetItem> result;
            bool success = TryConvertList(sourceValue, out result);

            targetValue = result;
            return success;
        }

        bool IConverter<string, ICollection<TTargetItem>>.TryConvert(string sourceValue, out ICollection<TTargetItem> targetValue)
        {
            List<TTargetItem> result;
            bool success = TryConvertList(sourceValue, out result);

            targetValue = result;
            return success;
        }

        bool IConverter<string, IEnumerable<TTargetItem>>.TryConvert(string sourceValue, out IEnumerable<TTargetItem> targetValue)
        {
            List<TTargetItem> result;
            bool success = TryConvertList(sourceValue, out result);

            targetValue = result;
            return success;
        }
    }
}
