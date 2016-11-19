using Neptuo.Logging.Serialization.Filters;
using Neptuo.Logging.Serialization.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging.Serialization
{
    /// <summary>
    /// Base implementation of <see cref="ILogSerializer"/> for text serializers.
    /// Uses <see cref="ILogFormatter"/> for formatting messages and <see cref="ILogFilter"/> for filtering scopes and levels.
    /// </summary>
    public abstract class TextLogSerializerBase : ILogSerializer
    {
        /// <summary>
        /// Log entry formatter.
        /// </summary>
        protected ILogFormatter Formatter { get; private set; }

        /// <summary>
        /// Log entry filter.
        /// </summary>
        protected ILogFilter Filter { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="formatter">Log entry formatter.</param>
        /// <param name="filter">Log entry filter.</param>
        protected TextLogSerializerBase(ILogFormatter formatter, ILogFilter filter)
        {
            Ensure.NotNull(formatter, "formatter");
            Ensure.NotNull(filter, "filter");
            Formatter = formatter;
            Filter = filter;
        }

        public void Append(string scopeName, LogLevel level, object model)
        {
            if (IsEnabled(scopeName, level))
            {
                string message = Formatter.Format(scopeName, level, model);
                Append(scopeName, level, message);
            }
        }

        /// <summary>
        /// Should append formatter message to the enabled scope and level.
        /// Parameter <paramref name="message" /> contains all information to log, based on configuration of <see cref="TextLogSerializerBase.Formatter"/>.
        /// </summary>
        /// <param name="scopeName">Scope name to write into.</param>
        /// <param name="level">Log message level.</param>
        /// <param name="message">Log message.</param>
        protected abstract void Append(string scopeName, LogLevel level, string message);

        public bool IsEnabled(string scopeName, LogLevel level)
        {
            return Filter.IsEnabled(scopeName, level);
        }
    }
}
