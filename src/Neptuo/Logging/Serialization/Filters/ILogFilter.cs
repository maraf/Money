using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging.Serialization.Filters
{
    /// <summary>
    /// Enables filtering by scope name and log level.
    /// </summary>
    public interface ILogFilter
    {
        /// <summary>
        /// Returns <c>true</c> if combination <paramref name="scopeName" /> and <paramref name="level"/> is enabled; otherwise <c>false</c>.
        /// </summary>
        /// <param name="scopeName">Log scope.</param>
        /// <param name="level">Log message level.</param>
        /// <returns><c>true</c> if combination <paramref name="scopeName" /> and <paramref name="level"/> is enabled; otherwise <c>false</c>.</returns>
        bool IsEnabled(string scopeName, LogLevel level);
    }
}
