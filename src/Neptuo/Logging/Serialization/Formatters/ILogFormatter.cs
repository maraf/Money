using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging.Serialization.Formatters
{
    /// <summary>
    /// ToString formatter for log entry.
    /// </summary>
    public interface ILogFormatter
    {
        /// <summary>
        /// Prepares passed parameters to string.
        /// </summary>
        /// <param name="scopeName">Scope name to write into.</param>
        /// <param name="level">Log message level.</param>
        /// <param name="model">Log message.</param>
        /// <returns>String representation of <see cref="scopeName"/>, <paramref name="level"/> and <paramref name="model"/>.</returns>
        string Format(string scopeName, LogLevel level, object model);
    }
}
