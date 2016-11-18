using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// Base implementation of <see cref="IConverter{TSource, TTarget}"/>.
    /// Supports converting to two way:
    /// 1) By passing delegate of type <see cref="OutFunc{TSource, TTarget, T}"/>.
    /// 2) By overring method <see cref="IConverter{TSource, TTarget}.TryConvert"/>.
    /// </summary>
    /// <typeparam name="TSource">Type of source value.</typeparam>
    /// <typeparam name="TTarget">Type of target value.</typeparam>
    public class DefaultConverter<TSource, TTarget> : IConverter<TSource, TTarget>
    {
        private readonly OutFunc<TSource, TTarget, bool> tryConvert;

        /// <summary>
        /// Creates new instance that requires overriding <see cref="IConverter{TSource, TTarget}.TryConvert"/>.
        /// </summary>
        public DefaultConverter()
        { }

        /// <summary>
        /// Creates new instance that converts by <paramref name="tryConvert"/>.
        /// </summary>
        /// <param name="tryConvert">Delegate for conversion of <typeparamref name="TSource"/> to <typeparamref name="TTarget"/>.</param>
        public DefaultConverter(OutFunc<TSource, TTarget, bool> tryConvert)
        {
            Ensure.NotNull(tryConvert, "converter");
            this.tryConvert = tryConvert;
        }

        public virtual bool TryConvert(TSource sourceValue, out TTarget targetValue)
        {
            if (tryConvert != null)
                return tryConvert(sourceValue, out targetValue);

            throw Ensure.Exception.InvalidOperation("Override TryConvert method or provider Converter function.");
        }

        public bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            Ensure.NotNull(sourceType, "sourceType");
            Ensure.NotNull(targetType, "targetType");

            TTarget target;
            if (TryConvert((TSource)sourceValue, out target))
            {
                targetValue = target;
                return true;
            }

            targetValue = null;
            return false;
        }
    }
}
