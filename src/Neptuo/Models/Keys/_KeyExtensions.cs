using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Keys
{
    /// <summary>
    /// The extensions for converting <see cref="IKey"/>.
    /// </summary>
    public static class _KeyExtensions
    {
        /// <summary>
        /// Tries to convert/cast <paramref name="key"/> to the type <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TKey">The requested type of key.</typeparam>
        /// <param name="key">The key to convert/cast.</param>
        /// <returns>The converted/casted key.</returns>
        /// <exception cref="RequiredKeyOfClassException">When the <paramref name="key"/> is not of the <typeparamref name="TKey"/>.</exception>
        public static TKey As<TKey>(this IKey key)
            where TKey : class, IKey
        {
            TKey target = key as TKey;
            if (target == null)
                throw new RequiredKeyOfClassException(key.GetType(), typeof(TKey));

            return target;
        }

        /// <summary>
        /// Tries to convert/cast <paramref name="key"/> to the <see cref="GuidKey"/>.
        /// </summary>
        /// <param name="key">The key to convert/cast.</param>
        /// <returns>The converted/casted key.</returns>
        /// <exception cref="RequiredKeyOfClassException">When the <paramref name="key"/> is not of the <see cref="GuidKey"/>.</exception>
        public static GuidKey AsGuidKey(this IKey key)
        {
            return As<GuidKey>(key);
        }

        /// <summary>
        /// Tries to convert/cast <paramref name="key"/> to the <see cref="Int16Key"/>.
        /// </summary>
        /// <param name="key">The key to convert/cast.</param>
        /// <returns>The converted/casted key.</returns>
        /// <exception cref="RequiredKeyOfClassException">When the <paramref name="key"/> is not of the <see cref="Int16Key"/>.</exception>
        public static Int16Key AsInt16Key(this IKey key)
        {
            return As<Int16Key>(key);
        }

        /// <summary>
        /// Tries to convert/cast <paramref name="key"/> to the <see cref="Int32Key"/>.
        /// </summary>
        /// <param name="key">The key to convert/cast.</param>
        /// <returns>The converted/casted key.</returns>
        /// <exception cref="RequiredKeyOfClassException">When the <paramref name="key"/> is not of the <see cref="Int32Key"/>.</exception>
        public static Int32Key AsInt32Key(this IKey key)
        {
            return As<Int32Key>(key);
        }

        /// <summary>
        /// Tries to convert/cast <paramref name="key"/> to the <see cref="Int64Key"/>.
        /// </summary>
        /// <param name="key">The key to convert/cast.</param>
        /// <returns>The converted/casted key.</returns>
        /// <exception cref="RequiredKeyOfClassException">When the <paramref name="key"/> is not of the <see cref="Int64Key"/>.</exception>
        public static Int64Key AsInt64Key(this IKey key)
        {
            return As<Int64Key>(key);
        }

        /// <summary>
        /// Tries to convert/cast <paramref name="key"/> to the <see cref="StringKey"/>.
        /// </summary>
        /// <param name="key">The key to convert/cast.</param>
        /// <returns>The converted/casted key.</returns>
        /// <exception cref="RequiredKeyOfClassException">When the <paramref name="key"/> is not of the <see cref="StringKey"/>.</exception>
        public static StringKey AsStringKey(this IKey key)
        {
            return As<StringKey>(key);
        }
    }
}
