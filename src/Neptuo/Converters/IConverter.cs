using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// Contract for generic converter.
    /// Supports converting multi source and targe types.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/>, of type <paramref name="sourceType"/>, to <paramref name="targeType"/>.
        /// </summary>
        /// <param name="sourceType">Source value type.</param>
        /// <param name="targetType">Target  value type.</param>
        /// <param name="sourceValue">Source value.</param>
        /// <param name="targetValue">Target value.</param>
        /// <returns><c>true</c> if <paramref name="sourceValue"/> can be converted to <paramref name="targetType"/>; <c>false</c> otherwise.</returns>
        bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue);
    }

    /// <summary>
    /// Single source to target type converter.
    /// </summary>
    /// <typeparam name="TSource">Type of source value.</typeparam>
    /// <typeparam name="TTarget">Type of target value.</typeparam>
    public interface IConverter<in TSource, TTarget> : IConverter
    {
        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/>, of type <typeparamref name="TSource"/>, to <typeparamref name="TTarget"/>.
        /// </summary>
        /// <param name="sourceValue">Source value.</param>
        /// <param name="targetValue">Target value.</param>
        /// <returns><c>true</c> if <paramref name="sourceValue"/> can be converted to <typeparamref name="TTarget"/>; <c>false</c> otherwise.</returns>
        bool TryConvert(TSource sourceValue, out TTarget targetValue);
    }
}
