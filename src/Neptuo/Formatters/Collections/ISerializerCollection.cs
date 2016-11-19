using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Collections
{
    /// <summary>
    /// The collection of registered serializers by <typeparamref name="TKey"/>.
    /// </summary>
    public interface ISerializerCollection<TKey> : ISerializerProvider<TKey>
    {
        /// <summary>
        /// Adds <paramref name="key"/> to be serialized by <paramref name="serializer"/>.
        /// </summary>
        /// <param name="key">The key to register <paramref name="serializer"/> with.</param>
        /// <param name="serializer">The serializer to register.</param>
        /// <returns>Self (for fluency).</returns>
        ISerializerCollection<TKey> Add(TKey key, ISerializer serializer);
    }
}
