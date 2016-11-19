using Neptuo.Logging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging
{
    /// <summary>
    /// Factory for child scopes.
    /// </summary>
    public interface ILogFactory
    {
        /// <summary>
        /// Dot separated log scope.
        /// </summary>
        string ScopeName { get; }

        /// <summary>
        /// Creates child log with <paramref name="scopeName"/> in it's scope path.
        /// </summary>
        /// <param name="scopeName"></param>
        /// <returns></returns>
        ILog Scope(string scopeName);

        /// <summary>
        /// Adds <paramref name="serializer"/> to be used for child scopes.
        /// </summary>
        /// <param name="serializer">Log item serializer.</param>
        /// <returns>Self (for fluency).</returns>
        ILogFactory AddSerializer(ILogSerializer serializer);
    }
}
