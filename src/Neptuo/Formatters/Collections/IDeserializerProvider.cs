using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Collections
{
    /// <summary>
    /// The provider of registered deserializers by <typeparamref name="TKey"/>.
    /// </summary>
    public interface IDeserializerProvider<TKey>
    {
        /// <summary>
        /// Tries to get deserializer registered with <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to find deserializer registered with.</param>
        /// <param name="deserializer">The deserializer registered with <paramref name="key"/>.</param>
        /// <returns><c>true</c> if <paramref name="deserializer"/> was found; <c>false</c> otherwise.</returns>
        bool TryGet(TKey key, out IDeserializer deserializer);
    }
}
