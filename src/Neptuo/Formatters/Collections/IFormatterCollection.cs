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
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface IFormatterCollection<TKey> : IFormatterProvider<TKey>
    {
        /// <summary>
        /// Adds <paramref name="key"/> to be serialized by <paramref name="formatter"/>.
        /// </summary>
        /// <param name="key">The key to register <paramref name="formatter"/> with.</param>
        /// <param name="formatter">The deserializer to register.</param>
        /// <returns>Self (for fluency).</returns>
        IFormatterCollection<TKey> Add(TKey key, IFormatter formatter);
    }
}
