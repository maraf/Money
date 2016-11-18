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
    /// Enumerator for <see cref="NameValueDictionary"/> or <see cref="NameValueReadOnlyDictionary"/>.
    /// </summary>
    internal class NameValueEnumerator : IEnumerator<KeyValuePair<string, string>>
    {
        private readonly NameValueCollection source;
        private readonly IEnumerator keyEnumerator;
        private KeyValuePair<string, string>? currrent;

        public NameValueEnumerator(NameValueCollection source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
            this.keyEnumerator = source.AllKeys.GetEnumerator();
        }

        private string GetCurrentKey()
        {
            return (string)keyEnumerator.Current;
        }

        public KeyValuePair<string, string> Current
        {
            get
            {
                if (currrent == null)
                {
                    if (keyEnumerator.Current != null)
                        currrent = new KeyValuePair<string, string>(GetCurrentKey(), source[GetCurrentKey()]);
                }
                return currrent.Value;
            }
        }

        public void Dispose()
        { }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            currrent = null;
            return keyEnumerator.MoveNext();
        }

        public void Reset()
        {
            currrent = null;
            keyEnumerator.Reset();
        }
    }
}
