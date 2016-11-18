using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// Registration extensions for <see cref="IConverterRepository"/>.
    /// </summary>
    public static class _ConverterRepositoryExtensions
    {
        /// <summary>
        /// Adds <paramref name="converter"/> for conversion from <typeparamref name="TSource"/> to <typeparamref name="TTarget"/>.
        /// </summary>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <param name="repository">The repository to register converter to.</param>
        /// <param name="converter">The converter.</param>
        /// <returns><paramref name="repository"/>.</returns>
        public static IConverterRepository Add<TSource, TTarget>(this IConverterRepository repository, IConverter<TSource, TTarget> converter)
        {
            Ensure.NotNull(repository, "repository");
            repository.Add(typeof(TSource), typeof(TTarget), converter);
            return repository;
        }

        /// <summary>
        /// Adds <paramref name="converter"/> for conversion from <typeparamref name="TOne"/> to <typeparamref name="TTwo"/> and from <typeparamref name="TTwo"/> to <typeparamref name="TOne"/>.
        /// </summary>
        /// <typeparam name="TOne">The first type of the vice-versa convertion.</typeparam>
        /// <typeparam name="TTwo">The second type of the vice-versa convertion.</typeparam>
        /// <param name="repository">The repository to register converter to.</param>
        /// <param name="converter">The converter.</param>
        /// <returns><paramref name="repository"/>.</returns>
        public static IConverterRepository Add<TOne, TTwo>(this IConverterRepository repository, TwoWayConverter<TOne, TTwo> converter)
        {
            DefaultConverter<TOne, TTwo> converter1 = converter;
            DefaultConverter<TTwo, TOne> converter2 = converter;
            Add<TOne, TTwo>(repository, converter1);
            Add<TTwo, TOne>(repository, converter2);
            return repository;
        }

        /// <summary>
        /// Adds <paramref name="converter"/> for conversion from <typeparamref name="TSource"/> to <typeparamref name="TTarget"/>.
        /// The <paramref name="converter"/> should always success.
        /// </summary>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <param name="repository">The repository to register converter to.</param>
        /// <param name="converter">The converter.</param>
        /// <returns><paramref name="repository"/>.</returns>
        public static IConverterRepository Add<TSource, TTarget>(this IConverterRepository repository, Func<TSource, TTarget> converter)
        {
            Ensure.NotNull(converter, "converter");
            return Add<TSource, TTarget>(repository, (TSource source, out TTarget target) => 
            {
                target = converter(source);
                return true;
            });
        }

        /// <summary>
        /// Adds <paramref name="tryConvert"/> for conversion from <typeparamref name="TSource"/> to <typeparamref name="TTarget"/>.
        /// </summary>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <param name="repository">The repository to register converter to.</param>
        /// <param name="tryConvert">The converter delegate.</param>
        /// <returns><paramref name="repository"/>.</returns>
        public static IConverterRepository Add<TSource, TTarget>(this IConverterRepository repository, OutFunc<TSource, TTarget, bool> tryConvert)
        {
            Ensure.NotNull(repository, "repository");
            Ensure.NotNull(tryConvert, "tryConvert");
            return Add(repository, new DefaultConverter<TSource, TTarget>(tryConvert));
        }

        /// <summary>
        /// Adds <paramref name="tryConvert"/> for conversion from <see cref="String"/> to <typeparamref name="TTarget"/>.
        /// Alse adds support for nullable conversion from <see cref="String"/> to <see cref="Nullable{TTarget}"/>.
        /// </summary>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <param name="repository">The repository to register converter to.</param>
        /// <param name="tryConvert">The converter delegate.</param>
        /// <param name="isWhitespaceAccepted">Whether whitespaces should be treated as nulls.</param>
        /// <returns><paramref name="repository"/>.</returns>
        public static IConverterRepository AddStringTo<TTarget>(this IConverterRepository repository, OutFunc<string, TTarget, bool> tryConvert, bool isWhitespaceAccepted = true)
            where TTarget : struct
        {
            Ensure.NotNull(repository, "repository");
            Ensure.NotNull(tryConvert, "tryConvert");

            IConverter<string, TTarget> converter = new DefaultConverter<string, TTarget>(tryConvert);
            return repository
                .Add<string, TTarget>(converter)
                .Add<string, TTarget?>(new StringToNullableConverter<TTarget>(converter, isWhitespaceAccepted));
        }

        /// <summary>
        /// Adds <paramref name="tryConvert"/> for convertion of string value separated by <paramref name="separator"/> 
        /// to <see cref="List{TTargetItem}"/>, <see cref="IList{TTargetItem}"/>, <see cref="ICollection{TTargetItem}"/> and <see cref="IEnumerable{TTargetItem}"/>.
        /// </summary>
        /// <typeparam name="TTargetItem">Type of collection item.</typeparam>
        /// <param name="repository">The repository to register converter to.</param>
        /// <param name="tryConvert">The converter delegate.</param>
        /// <param name="separator">Item separator.</param>
        /// <returns><paramref name="repository"/>.</returns>
        public static IConverterRepository AddStringToCollection<TTargetItem>(this IConverterRepository repository, OutFunc<string, TTargetItem, bool> tryConvert, string separator = ",")
        {
            Ensure.NotNull(repository, "repository");
            StringToCollectionConverter<TTargetItem> converter = new StringToCollectionConverter<TTargetItem>(separator, new DefaultConverter<string, TTargetItem>(tryConvert));
            return repository
                .Add<string, List<TTargetItem>>(converter)
                .Add<string, IList<TTargetItem>>(converter)
                .Add<string, ICollection<TTargetItem>>(converter)
                .Add<string, IEnumerable<TTargetItem>>(converter);
        }

        /// <summary>
        /// Adds converter for conversion from <typeparamref name="TSource"/> to <see cref="String"/>.
        /// If <paramref name="format"/> is not <c>null</c>, then it is used as format string, eg. yyyy-MM-dd.
        /// </summary>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <param name="repository">The repository to register converter to.</param>
        /// <param name="format">Format string, eg. yyyy-MM-dd for datetime.</param>
        /// <returns><paramref name="repository"/>.</returns>
        public static IConverterRepository AddToString<TSource>(this IConverterRepository repository, string format = null)
        {
            Ensure.NotNull(repository, "repository");
            if (format == null)
                return Add(repository, new ToStringConverter<TSource>());
            else
                return Add(repository, new ToStringConverter<TSource>(format));
        }

        /// <summary>
        /// Adds search handler for converting any type to string.
        /// </summary>
        /// <param name="repository">The repository to register converter to.</param>
        /// <returns><paramref name="repository"/>.</returns>
        public static IConverterRepository AddToStringSearchHandler(this IConverterRepository repository)
        {
            Ensure.NotNull(repository, "repository");
            return repository.AddSearchHandler(TryConvertToString);
        }

        private static bool TryConvertToString(ConverterSearchContext context, out IConverter converter)
        {
            converter = new ToStringConverter();
            return true;
        }

        /// <summary>
        /// Adds string to any enum (and any nullable enum) converter search handler.
        /// Any enums types will be converted using <see cref="StringToEnumConverter"/>.
        /// </summary>
        /// <param name="repository">The repository to register converter to.</param>
        /// <param name="isCaseSensitive">Whether parsing should be case-sensitive.</param>
        /// <returns><paramref name="repository"/>.</returns>
        public static IConverterRepository AddEnumSearchHandler(this IConverterRepository repository, bool isCaseSensitive)
        {
            Ensure.NotNull(repository, "repository");

            if (isCaseSensitive)
                return repository.AddSearchHandler(TryConvertStringToEnumCaseSensitive);
            else
                return repository.AddSearchHandler(TryConvertStringToEnum);
        }

        private static bool TryConvertStringToEnumCaseSensitive(ConverterSearchContext context, out IConverter converter)
        {
            if (context.TargetType.GetTypeInfo().IsEnum)
            {
                converter = new StringToEnumConverter(true);
                return true;
            }
            else if (context.TargetType.GetTypeInfo().IsGenericType && context.TargetType.GetGenericTypeDefinition() == typeof(Nullable<>) && context.TargetType.GetGenericArguments()[0].GetTypeInfo().IsEnum)
            {
                converter = new StringToNullableConverter(new StringToEnumConverter(false), false);
                return true;
            }

            converter = null;
            return false;
        }

        private static bool TryConvertStringToEnum(ConverterSearchContext context, out IConverter converter)
        {
            if (context.TargetType.GetTypeInfo().IsEnum)
            {
                converter = new StringToEnumConverter(false);
                return true;
            }
            else if (context.TargetType.GetTypeInfo().IsGenericType && context.TargetType.GetGenericTypeDefinition() == typeof(Nullable<>) && context.TargetType.GetGenericArguments()[0].GetTypeInfo().IsEnum)
            {
                converter = new StringToNullableConverter(new StringToEnumConverter(false), true);
                return true;
            }

            converter = null;
            return false;
        }
    }
}
