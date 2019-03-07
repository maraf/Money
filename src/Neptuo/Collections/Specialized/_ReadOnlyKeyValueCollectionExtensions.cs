using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Collections.Specialized
{
    /// <summary>
    /// Some common extensions.
    /// </summary>
    public static class _ReadOnlyKeyValueCollectionExtensions
    {
        /// <summary>
        /// Reads the value of <paramref name="key"/> in <paramref name="collection"/>.
        /// If value is found and can be converted to <typeparamref name="T"/>, returns it.
        /// Otherwise throws <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="collection">Collection of key-value pairs.</param>
        /// <param name="key">Requested key.</param>
        /// <param name="defaultValue">Optional default value if is not found or not convertible.</param>
        /// <returns>Value of <paramref name="key"/> in <paramref name="collection"/>.</returns>
        public static T Get<T>(this IReadOnlyKeyValueCollection collection, string key)
        {
            Ensure.NotNull(collection, "collection");

            T value;
            if (collection.TryGet(key, out value))
                return value;

            throw Ensure.Exception.InvalidOperation("Collection doesn't contain value of type '{0}' with key '{1}'.", typeof(T), key);
        }

        /// <summary>
        /// Reads the value of <paramref name="key"/> in <paramref name="collection"/>.
        /// If value is found and can be converted to <typeparamref name="T"/>, returns it.
        /// Otherwise returns <paramref name="defaultValue"/> (if provided) or throws <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="collection">Collection of key-value pairs.</param>
        /// <param name="key">Requested key.</param>
        /// <param name="defaultValue">Optional default value if is not found or not convertible.</param>
        /// <returns>Value of <paramref name="key"/> in <paramref name="collection"/>.</returns>
        public static T Get<T>(this IReadOnlyKeyValueCollection collection, string key, T defaultValue)
        {
            Ensure.NotNull(collection, "collection");

            T value;
            if (collection.TryGet(key, out value))
                return value;

            return defaultValue;
        }

        /// <summary>
        /// Reads the value of <paramref name="key"/> in <paramref name="collection"/>.
        /// If value is found and can be converted to <typeparamref name="T"/>, returns it.
        /// Otherwise the default value of <typeparamref name="T"/> is returned.
        /// </summary>
        /// <param name="collection">Collection of key-value pairs.</param>
        /// <param name="key">Requested key.</param>
        /// <returns>Value of <paramref name="key"/> in <paramref name="collection"/>.</returns>
        public static T Find<T>(this IReadOnlyKeyValueCollection collection, string key)
        {
            Ensure.NotNull(collection, "collection");

            T value;
            if (collection.TryGet(key, out value))
                return value;

            return default(T);
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="collection"/> contains <paramref name="key"/>.
        /// </summary>
        /// <param name="collection">Collection of key-value pairs.</param>
        /// <param name="key">Requested key.</param>
        /// <returns><c>true</c> if <paramref name="collection"/> contains <paramref name="key"/>; otherwise <c>false</c>.</returns>
        public static bool Has(this IReadOnlyKeyValueCollection collection, string key)
        {
            Ensure.NotNull(collection, "collection");
            Ensure.NotNull(key, "key");

            if (collection.Keys.Contains(key))
                return true;

            object value;
            return collection.TryGet(key, out value);
        }
    }
}