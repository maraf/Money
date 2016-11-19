using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Collections
{
    /// <summary>
    /// The collection of registered deserializers by <typeparamref name="TKey"/>.
    /// </summary>
    public interface IDeserializerCollection<TKey> : IDeserializerProvider<TKey>
    {
        /// <summary>
        /// Adds <paramref name="key"/> to be serialized by <paramref name="deserializer"/>.
        /// </summary>
        /// <param name="key">The key to register <paramref name="deserializer"/> with.</param>
        /// <param name="deserializer">The deserializer to register.</param>
        /// <returns>Self (for fluency).</returns>
        IDeserializerCollection<TKey> Add(TKey key, IDeserializer deserializer);
    }
}
