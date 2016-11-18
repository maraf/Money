using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// Implementation of converter from <see cref="string"/> to nullable version of <typeparamref name="TTarget"/>.
    /// 
    /// If string value can be converted using non-nullable converter, value is converted.
    /// If string value is <c>null</c> or <see cref="String.Empty"/>, value is set to <c>null</c> and <c>true</c> is returned; otherwise <c>false</c> is returned from conversion.
    /// Setting one of constructor parameters to <c>true</c>, whitespaces can also be treaten as <c>null</c>.
    /// 
    /// Inner converter is called the first, after that null checks are executed.
    /// </summary>
    /// <typeparam name="TTarget">Target type.</typeparam>
    public class StringToNullableConverter<TTarget> : DefaultConverter<string, TTarget?>
        where TTarget : struct
    {
        private readonly IConverter<string, TTarget> converter;
        private readonly bool isWhitespaceAccepted;

        /// <summary>
        /// Creates new instance that uses <paramref name="converter"/> for converting non-nullable value.
        /// </summary>
        /// <param name="converter">Inner non-nullable converter.</param>
        /// <param name="isWhitespaceAccepted">Whether whitespaces should be treated as nulls.</param>
        public StringToNullableConverter(IConverter<string, TTarget> converter, bool isWhitespaceAccepted)
        {
            Ensure.NotNull(converter, "converter");
            this.converter = converter;
            this.isWhitespaceAccepted  = isWhitespaceAccepted;
        }

        public override bool TryConvert(string sourceValue, out TTarget? targetValue)
        {
            TTarget value;
            if (converter.TryConvert(sourceValue, out value))
            {
                targetValue = value;
                return true;
            }

            if(String.IsNullOrEmpty(sourceValue))
            {
                targetValue = null;
                return true;
            }

            if (isWhitespaceAccepted && String.IsNullOrWhiteSpace(sourceValue))
            {
                targetValue = null;
                return true;
            }

            targetValue = null;
            return false;
        }
    }

    /// <summary>
    /// Implementation of converter from <see cref="string"/> to nullable version of <typeparamref name="TTarget"/>.
    /// 
    /// If string value can be converted using non-nullable converter, value is converted.
    /// If string value is <c>null</c> or <see cref="String.Empty"/>, value is set to <c>null</c> and <c>true</c> is returned; otherwise <c>false</c> is returned from conversion.
    /// Setting one of constructor parameters to <c>true</c>, whitespaces can also be treaten as <c>null</c>.
    /// 
    /// Inner converter is called the first, after that null checks are executed.
    /// This converter can be used generally for any type of nullable object.
    /// </summary>
    /// <typeparam name="TTarget">Target type.</typeparam>
    public class StringToNullableConverter : IConverter
    {
        private readonly IConverter converter;
        private readonly bool isWhitespaceAccepted;

        /// <summary>
        /// Creates new instance that uses <paramref name="converter"/> for converting non-nullable value.
        /// </summary>
        /// <param name="converter">Inner non-nullable converter.</param>
        /// <param name="isWhitespaceAccepted">Whether whitespaces should be treated as nulls.</param>
        public StringToNullableConverter(IConverter converter, bool isWhitespaceAccepted)
        {
            Ensure.NotNull(converter, "converter");
            this.converter = converter;
            this.isWhitespaceAccepted = isWhitespaceAccepted;
        }

        public bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            if (sourceType == typeof(string) && targetType.GetTypeInfo().IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type innerType = targetType.GetGenericArguments()[0];
                if (converter.TryConvert(sourceType, innerType, sourceValue, out targetValue))
                    return true;

                string stringSourceValue = (string)sourceValue;
                if (String.IsNullOrEmpty(stringSourceValue))
                {
                    targetValue = null;
                    return true;
                }

                if (isWhitespaceAccepted && String.IsNullOrWhiteSpace(stringSourceValue))
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
