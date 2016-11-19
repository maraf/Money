using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Collections
{
    /// <summary>
    /// Default implementation of <see cref="IFormatterCollection{TKey}"/>, <see cref="ISerializerProvider{TKey}"/> and <see cref="IDeserializerProvider{TKey}"/>.
    /// Writing to this collection is not thread-safe, reading is thread-safe.
    /// </summary>
    public class DefaultFormatterCollection<TKey> : IFormatterCollection<TKey>, ISerializerProvider<TKey>, IDeserializerProvider<TKey>
    {
        private readonly Dictionary<TKey, IFormatter> storage = new Dictionary<TKey, IFormatter>();

        public IFormatterCollection<TKey> Add(TKey key, IFormatter formatter)
        {
            Ensure.NotNull(key, "key");
            Ensure.NotNull(formatter, "formatter");
            storage[key] = formatter;
            return this;
        }

        public bool TryGet(TKey key, out IFormatter formatter)
        {
            Ensure.NotNull(key, "key");
            return storage.TryGetValue(key, out formatter);
        }

        public bool TryGet(TKey key, out ISerializer serializer)
        {
            IFormatter formatter;
            if (TryGet(key, out formatter))
            {
                serializer = formatter;
                return true;
            }

            serializer = null;
            return false;
        }

        public bool TryGet(TKey key, out IDeserializer deserializer)
        {
            IFormatter formatter;
            if (TryGet(key, out formatter))
            {
                deserializer = formatter;
                return true;
            }

            deserializer = null;
            return false;
        }
    }
}
