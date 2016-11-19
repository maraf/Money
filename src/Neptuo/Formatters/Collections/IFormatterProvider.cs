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
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface IFormatterProvider<TKey>
    {
        /// <summary>
        /// Tries to get deserializer registered with <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to find deserializer registered with.</param>
        /// <param name="formatter">The deserializer registered with <paramref name="key"/>.</param>
        /// <returns><c>true</c> if <paramref name="formatter"/> was found; <c>false</c> otherwise.</returns>
        bool TryGet(TKey key, out IFormatter formatter);
    }
}
