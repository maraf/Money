using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// Repository for ceonverters between types.
    /// </summary>
    public interface IConverterRepository
    {
        /// <summary>
        /// Adds <paramref name="converter"/> for conversion from <paramref name="sourceType"/> to <paramref name="targetType"/>.
        /// </summary>
        /// <param name="sourceType">Source type.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="converter">Converter.</param>
        /// <returns>Self (for fluency).</returns>
        IConverterRepository Add(Type sourceType, Type targetType, IConverter converter);

        /// <summary>
        /// Adds <paramref name="searchHandler"/> to be executed when converter was not found.
        /// </summary>
        /// <param name="searchHandler">Converter provider method.</param>
        /// <returns>Self (for fluency).</returns>
        IConverterRepository AddSearchHandler(OutFunc<ConverterSearchContext, IConverter, bool> searchHandler);

        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/> of type <typeparamref name="TSource"/> to target type <typeparamref name="TTarget"/>.
        /// </summary>
        /// <typeparam name="TSource">Source value type.</typeparam>
        /// <typeparam name="TTarget">Target value type.</typeparam>
        /// <param name="sourceValue">Source value.</param>
        /// <param name="targetValue">Output target value.</param>
        /// <returns>True if conversion was successfull.</returns>
        bool TryConvert<TSource, TTarget>(TSource sourceValue, out TTarget targetValue);

        /// <summary>
        /// Tries to convert <paramref name="sourceValue"/> to target type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="sourceType">Type of source value.</param>
        /// <param name="targetType">Type of target value.</param>
        /// <param name="sourceValue">Source value.</param>
        /// <param name="targetValue">Output target value.</param>
        /// <returns>True if conversion was successfull.</returns>
        bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue);


        /// <summary>
        /// Returns a function that can be reused to convert <typeparamref name="TSource"/> to <typeparamref name="TTarget"/>.
        /// If conversion is not supported (missing converter), the function exception is thrown.
        /// </summary>
        /// <typeparam name="TSource">Source value type.</typeparam>
        /// <typeparam name="TTarget">Target value type.</typeparam>
        /// <returns>A function that can be reused to convert <typeparamref name="TSource"/> to <typeparamref name="TTarget"/>.</returns>
        Func<TSource, TTarget> GetConverter<TSource, TTarget>();

        /// <summary>
        /// Returns a function that can be reused to convert <typeparamref name="TSource"/> to <typeparamref name="TTarget"/>.
        /// If conversion is not supported (missing converter) or not possible, the function returns <c>false</c>.
        /// </summary>
        /// <typeparam name="TSource">Source value type.</typeparam>
        /// <typeparam name="TTarget">Target value type.</typeparam>
        /// <returns>A function that can be reused to convert <typeparamref name="TSource"/> to <typeparamref name="TTarget"/>.</returns>
        OutFunc<TSource, TTarget, bool> GetTryConverter<TSource, TTarget>();
    }
}
