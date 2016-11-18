using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Collections.Specialized
{
    /// <summary>
    /// Key/value collection.
    /// </summary>
    public interface IKeyValueCollection : IReadOnlyKeyValueCollection
    {
        /// <summary>
        /// Associates <paramref name="value"/> with <paramref name="key"/> in collection.
        /// If collection already contains <paramref name="key"/>, implementation should override current values with <paramref name="value"/>.
        /// </summary>
        /// <param name="key">Key to associate <paramref name="value"/> with.</param>
        /// <param name="value">New value of <paramref name="key"/>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="key"/> is <c>null</c>.</exception>
        IKeyValueCollection Add(string key, object value);
    }
}
