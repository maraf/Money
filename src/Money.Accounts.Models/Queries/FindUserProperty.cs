using Neptuo;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// Finds an user's value for property with key.
    /// </summary>
    public class FindUserProperty : UserQuery, IQuery<string>
    {
        /// <summary>
        /// Gets a property key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Creates a new instance that finds an user's value for property with <paramref name="key"/>.
        /// </summary>
        /// <param name="key">A property key.</param>
        public FindUserProperty(string key)
        {
            Ensure.NotNull(key, "key");
            Key = key;
        }
    }
}
