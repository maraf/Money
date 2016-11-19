using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Repositories
{
    /// <summary>
    /// Exception raised when to such aggregate root exists.
    /// </summary>
    public class AggregateRootNotFoundException : AggregateRootException
    {
        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="key">The key of the aggregate root which was not found in the repository.</param>
        public AggregateRootNotFoundException(IKey key)
        {
            Key = key;
        }
    }
}
