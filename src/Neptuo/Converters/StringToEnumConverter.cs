using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// Implementation of <see cref="IConverter"/> for converting string to enum types.
    /// </summary>
    public class StringToEnumConverter : IConverter
    {
        private readonly bool isCaseSensitive;

        /// <summary>
        /// Create instance with case-sensitive value parsing.
        /// </summary>
        public StringToEnumConverter()
            : this(false)
        { }

        /// <summary>
        /// Creates instance with <paramref name="isCaseSensitive"/> for setting case-sensitive value parsing.
        /// </summary>
        /// <param name="isCaseSensitive">Whether parsing should be case-sensitive.</param>
        public StringToEnumConverter(bool isCaseSensitive)
        {
            this.isCaseSensitive = isCaseSensitive;
        }

        public bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            if (sourceType == typeof(string) && targetType.GetTypeInfo().IsEnum)
            {
                string stringSourceValue = (string)sourceValue;
                if (String.IsNullOrEmpty(stringSourceValue))
                {
                    targetValue = null;
                    return false;
                }

                try
                {
                    targetValue = Enum.Parse(targetType, stringSourceValue, !isCaseSensitive);
                    return true;
                }
                catch (Exception)
                {
                    targetValue = null;
                    return false;
                }
            }

            targetValue = null;
            return false;
        }
    }
}
