using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Collections
{
    /// <summary>
    /// Default implementation of <see cref="IDeserializerCollection{TKey}"/>.
    /// Writing to this collection is not thread-safe, reading is thread-safe.
    /// </summary>
    public class DefaultDeserializerCollection<TKey> : IDeserializerCollection<TKey>
    {
        private readonly Dictionary<TKey, IDeserializer> storage = new Dictionary<TKey, IDeserializer>();

        public IDeserializerCollection<TKey> Add(TKey key, IDeserializer formatter)
        {
            Ensure.NotNull(key, "key");
            Ensure.NotNull(formatter, "formatter");
            storage[key] = formatter;
            return this;
        }

        public bool TryGet(TKey key, out IDeserializer formatter)
        {
            Ensure.NotNull(key, "key");
            return storage.TryGetValue(key, out formatter);
        }
    }
}
