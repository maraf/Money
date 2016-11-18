using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Collections.Specialized
{
    /// <summary>
    /// Wraps instance of <see cref="NameValueCollection"/> as <see cref="IReadOnlyDictionary"/>.
    /// </summary>
    /// <remarks>
    /// Only CopyTo is not implemented!
    /// </remarks>
    public class NameValueDictionary : IDictionary<string, string>
    {
        /// <summary>
        /// Source values collection.
        /// </summary>
        private readonly NameValueCollection source;

        /// <summary>
        /// Cached all source values.
        /// </summary>
        private List<string> allValues;

        /// <summary>
        /// Creates new instance with <see cref="source"/> as source values collection.
        /// </summary>
        /// <param name="source">Source values collection</param>
        public NameValueDictionary(NameValueCollection source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
        }

        public void Add(string key, string value)
        {
            Ensure.NotNull(key, "key");
            if (ContainsKey(key))
                throw Ensure.Exception.Argument("key", "Collection already contains it with key '{0}'.", key);

            source[key] = value;
        }

        public bool ContainsKey(string key)
        {
            Ensure.NotNull(key, "key");
            return source.AllKeys.Contains(key);
        }

        public ICollection<string> Keys
        {
            get { return source.AllKeys; }
        }

        public bool Remove(string key)
        {
            Ensure.NotNull(key, "key");
            if (ContainsKey(key))
            {
                source.Remove(key);
                return true;
            }
            return false;
        }

        public bool TryGetValue(string key, out string value)
        {
            if (ContainsKey(key))
            {
                value = source[key];
                return true;
            }

            value = null;
            return false;
        }

        public ICollection<string> Values
        {
            get
            {
                // Caches all values from source.
                if(allValues == null)
                {
                    allValues = new List<string>();
                    foreach (string key in source.AllKeys)
                        allValues.Add(source[key]);
                }
                return allValues;
            }
        }

        public string this[string key]
        {
            get
            {
                Ensure.NotNullOrEmpty(key, "key");
                return source[key];
            }
            set
            {
                Ensure.NotNull(key, "key");
                source[key] = value;
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            Ensure.NotNull(item, "item");
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            source.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            Ensure.NotNull(item, "item");

            string value;
            if (TryGetValue(item.Key, out value))
                return value == item.Value;

            return false;
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw Ensure.Exception.NotImplemented();
        }

        public int Count
        {
            get { return source.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            string value;
            if (TryGetValue(item.Key, out value) && value == item.Value)
            {
                source.Remove(item.Key);
                return true;
            }
            return false;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return new NameValueEnumerator(source);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new NameValueEnumerator(source);
        }
    }
}
