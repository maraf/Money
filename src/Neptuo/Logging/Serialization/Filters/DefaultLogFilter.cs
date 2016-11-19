using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging.Serialization.Filters
{
    /// <summary>
    /// Default implementation of <see cref="ILogFilter"/>.
    /// </summary>
    public class DefaultLogFilter : ILogFilter
    {
        private readonly HashSet<string> scopeNames;
        private readonly LogLevel minLevel;

        /// <summary>
        /// Creates new instance filtered by <paramref name="scopeNames"/> and minimal log level <paramref name="minLevel"/>.
        /// </summary>
        /// <param name="scopeNames">Enabled scope names. Instance is used.</param>
        /// <param name="minLevel">Minimal enabled log level (including).</param>
        public DefaultLogFilter(HashSet<string> scopeNames, LogLevel minLevel)
        {
            Ensure.NotNull(scopeNames, "scopeNames");
            this.scopeNames = scopeNames;
            this.minLevel = minLevel;
        }

        /// <summary>
        /// Creates new instance filtered by <paramref name="scopeNames"/> and minimal log level <paramref name="minLevel"/>.
        /// </summary>
        /// <param name="scopeNames">Enabled scope names. Items are copied.</param>
        /// <param name="minLevel">Minimal enabled log level (including).</param>
        public DefaultLogFilter(IEnumerable<string> scopeNames, LogLevel minLevel)
            : this(new HashSet<string>(scopeNames), minLevel)
        { }

        /// <summary>
        /// Creates new instance filtered by minimal log level <paramref name="minLevel"/>.
        /// </summary>
        /// <param name="minLevel">Minimal enabled log level (including).</param>
        public DefaultLogFilter(LogLevel minLevel)
        {
            this.minLevel = minLevel;
        }

        public bool IsEnabled(string scopeName, LogLevel level)
        {
            if (level < minLevel)
                return false;

            if (scopeNames == null)
                return true;

            return scopeNames.Contains(scopeName);
        }

        /// <summary>
        /// Logs all messages.
        /// </summary>
        public static readonly DefaultLogFilter Debug = new DefaultLogFilter(LogLevel.Debug);

        /// <summary>
        /// Logs only <see cref="LogLevel.Info"/> and higher.
        /// </summary>
        public static readonly DefaultLogFilter Info = new DefaultLogFilter(LogLevel.Info);

        /// <summary>
        /// Logs only <see cref="LogLevel.Warning"/> and higher.
        /// </summary>
        public static readonly DefaultLogFilter Warning = new DefaultLogFilter(LogLevel.Warning);

        /// <summary>
        /// Logs only <see cref="LogLevel.Error"/> and higher.
        /// </summary>
        public static readonly DefaultLogFilter Error = new DefaultLogFilter(LogLevel.Error);

        /// <summary>
        /// Logs only <see cref="LogLevel.Fatal"/>.
        /// </summary>
        public static readonly DefaultLogFilter Fatal = new DefaultLogFilter(LogLevel.Fatal);
    }
}
