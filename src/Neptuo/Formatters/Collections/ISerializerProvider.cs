using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Collections
{
    /// <summary>
    /// The provider of registered serializers by <typeparamref name="TKey"/>.
    /// </summary>
    public interface ISerializerProvider<TKey>
    {
        /// <summary>
        /// Tries to get serializer registered with <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to find serializer registered with.</param>
        /// <param name="serializer">The serializer registered with <paramref name="key"/>.</param>
        /// <returns><c>true</c> if <paramref name="serializer"/> was found; <c>false</c> otherwise.</returns>
        bool TryGet(TKey key, out ISerializer serializer);
    }
}
