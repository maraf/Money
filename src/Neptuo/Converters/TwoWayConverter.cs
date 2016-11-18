using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// The implementation of <see cref="IConverter"/> with support for converting from <typeparamref name="TOne"/> to <typeparamref name="TTwo"/> and vice-versa.
    /// </summary>
    /// <typeparam name="TOne">The first type of the vice-versa convertion.</typeparam>
    /// <typeparam name="TTwo">The second type of the vice-versa convertion.</typeparam>
    public abstract class TwoWayConverter<TOne, TTwo> : IConverter
    {
        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/>, of type <typeparamref name="TOne"/>, to <typeparamref name="TTwo"/>.
        /// </summary>
        /// <param name="sourceValue">Source value.</param>
        /// <param name="targetValue">Target value.</param>
        /// <returns><c>true</c> if <paramref name="sourceValue"/> can be converted to <typeparamref name="TTwo"/>; <c>false</c> otherwise.</returns>
        public abstract bool TryConvertFromOneToTwo(TOne sourceValue, out TTwo targetValue);

        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/>, of type <typeparamref name="TTwo"/>, to <typeparamref name="TOne"/>.
        /// </summary>
        /// <param name="sourceValue">Source value.</param>
        /// <param name="targetValue">Target value.</param>
        /// <returns><c>true</c> if <paramref name="sourceValue"/> can be converted to <typeparamref name="TOne"/>; <c>false</c> otherwise.</returns>
        public abstract bool TryConvertFromTwoToOne(TTwo sourceValue, out TOne targetValue);

        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/> to te type <paramref name="targetType"/> if 
        /// source value is of type <typeparamref name="TOne"/> or <typeparamref name="TTwo"/> and the target type is the other.
        /// </summary>
        /// <param name="sourceType">The type of the source value.</param>
        /// <param name="targetType">The target type to convert the source value to.</param>
        /// <param name="sourceValue">The source value.</param>
        /// <param name="targetValue">The target value or <c>null</c>.</param>
        /// <returns><c>true</c> if conversion was provided; <c>false</c> otherwise.</returns>
        public bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            if (sourceType == typeof(TOne) && targetType == typeof(TTwo))
            {
                TOne source = (TOne)sourceValue;
                TTwo target;
                if (TryConvertFromOneToTwo(source, out target))
                {
                    targetValue = target;
                    return true;
                }
            }
            else if (sourceType == typeof(TTwo) && targetType == typeof(TOne))
            {
                TTwo source = (TTwo)sourceValue;
                TOne target;
                if(TryConvertFromTwoToOne(source, out target))
                {
                    targetValue = target;
                    return true;
                }
            }

            targetValue = null;
            return false;
        }

        /// <summary>
        /// Converts to default generic converter (also implemeting <see cref="IConverter{T1, T2}"/>) from <typeparamref name="TOne"/> to <typeparamref name="TTwo"/>.
        /// </summary>
        /// <param name="instance">The instance to use as conversion source.</param>
        /// <returns>The default generic converter.</returns>
        public static implicit operator DefaultConverter<TOne, TTwo>(TwoWayConverter<TOne, TTwo> instance)
        {
            return new DefaultConverter<TOne, TTwo>(instance.TryConvertFromOneToTwo);
        }

        /// <summary>
        /// Converts to default generic converter (also implemeting <see cref="IConverter{T1, T2}"/>) from <typeparamref name="TTwo"/> to <typeparamref name="TOne"/>.
        /// </summary>
        /// <param name="instance">The instance to use as conversion source.</param>
        /// <returns>The default generic converter.</returns>
        public static implicit operator DefaultConverter<TTwo, TOne>(TwoWayConverter<TOne, TTwo> instance)
        {
            return new DefaultConverter<TTwo, TOne>(instance.TryConvertFromTwoToOne);
        }
    }
}
