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
    public class NameValueReadOnlyDictionary : IReadOnlyDictionary<string, string>
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
        public NameValueReadOnlyDictionary(NameValueCollection source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
        }

        public bool ContainsKey(string key)
        {
            Ensure.NotNull(key, "key");
            return source.AllKeys.Contains(key);
        }

        public IEnumerable<string> Keys
        {
            get { return source.AllKeys; }
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

        public IEnumerable<string> Values
        {
            get
            {
                // Caches all values from source.
                if (allValues == null)
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
        }

        public int Count
        {
            get { return source.Count; }
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
