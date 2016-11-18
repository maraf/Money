using Neptuo.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// Singleton utilility for converting between types.
    /// </summary>
    public static class Converts
    {
        private static object lockRepository = new object();
        private static IConverterRepository repository;

        /// <summary>
        /// Singleton converter repository.
        /// </summary>
        public static IConverterRepository Repository
        {
            get
            {
                if (repository == null)
                {
                    lock (lockRepository)
                    {
                        if (repository == null)
                            repository = new DefaultConverterRepository();
                    }
                }
                return repository;
            }
        }

        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/> of type <typeparamref name="TSource"/> to target type <typeparamref name="TTarget"/>.
        /// </summary>
        /// <typeparam name="TSource">Source value type.</typeparam>
        /// <typeparam name="TTarget">Target value type.</typeparam>
        /// <param name="sourceValue">Source value.</param>
        /// <param name="targetValue">Output target value.</param>
        /// <returns>True if conversion was successfull.</returns>
        public static bool Try<TSource, TTarget>(TSource sourceValue, out TTarget targetValue)
        {
            return Repository.TryConvert(sourceValue, out targetValue);
        }

        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/> to target type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="sourceType">Type of source value.</param>
        /// <param name="targetType">Type of target value.</param>
        /// <param name="sourceValue">Source value.</param>
        /// <param name="targetValue">Output target value.</param>
        /// <returns>True if conversion was successfull.</returns>
        public static bool Try(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            return Repository.TryConvert(sourceType, targetType, sourceValue, out targetValue);
        }

        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/> of type <typeparamref name="TSource"/> to target type <typeparamref name="TTarget"/>.
        /// If conversion is not possible, throws exception <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <typeparam name="TSource">Source value type.</typeparam>
        /// <typeparam name="TTarget">Target value type.</typeparam>
        /// <param name="sourceValue">Source value.</param>
        /// <returns>Value <paramref name="sourceValue" /> converted to type <typeparamref name="TTarget" /></returns>
        public static TTarget To<TSource, TTarget>(TSource sourceValue)
        {
            TTarget targetValue;
            if (Try(sourceValue, out targetValue))
                return targetValue;

            throw Ensure.Exception.ArgumentOutOfRange("TTarget", "Target type '{0}' can't constructed from value '{1}'.", typeof(TTarget).FullName, sourceValue);
        }

        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/> to target type <paramref name="targetType"/>.
        /// If conversion is not possible, throws exception <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <param name="sourceType">Type of source value.</param>
        /// <param name="targetType">Type of target value.</param>
        /// <param name="sourceValue">Source value.</param>
        /// <returns>Value <paramref name="sourceValue" /> converted to type <paramref name="targetType"/>.</returns>
        public static object To(Type sourceType, Type targetType, object sourceValue)
        {
            object targetValue;
            if (Try(sourceType, targetType, sourceValue, out targetValue))
                return targetValue;

            Ensure.NotNull(targetType, "targetType");
            throw Ensure.Exception.ArgumentOutOfRange("TTarget", "Target type '{0}' can't constructed from value '{1}'.", targetType.FullName, sourceValue);
        }

        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/> to target type <paramref name="targetType"/>.
        /// If conversion is not possible, throws <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <param name="targetType">Type of target value.</param>
        /// <param name="sourceValue">Source value.</param>
        /// <returns>Value <paramref name="sourceValue" /> converted to type <paramref name="targetType"/>.</returns>
        public static object To(Type targetType, object sourceValue)
        {
            if (sourceValue == null)
            {
                if (targetType.GetTypeInfo().IsValueType)
                    return Activator.CreateInstance(targetType);

                return null;
            }

            return To(sourceValue.GetType(), targetType, sourceValue);
        }
    }
}
