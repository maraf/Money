using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Collections
{
    /// <summary>
    /// Default implementation of <see cref="ISerializerCollection{TKey}"/>.
    /// Writing to this collection is not thread-safe, reading is thread-safe.
    /// </summary>
    public class DefaultSerializerCollection<TKey> : ISerializerCollection<TKey>
    {
        private readonly Dictionary<TKey, ISerializer> storage = new Dictionary<TKey, ISerializer>();

        public ISerializerCollection<TKey> Add(TKey key, ISerializer formatter)
        {
            Ensure.NotNull(key, "key");
            Ensure.NotNull(formatter, "formatter");
            storage[key] = formatter;
            return this;
        }

        public bool TryGet(TKey key, out ISerializer formatter)
        {
            Ensure.NotNull(key, "key");
            return storage.TryGetValue(key, out formatter);
        }
    }
}
