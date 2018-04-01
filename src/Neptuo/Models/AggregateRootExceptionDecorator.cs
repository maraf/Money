using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models
{
    /// <summary>
    /// Provides methods for setting <see cref="AggregateRootException"/> command and aggregate relate keys.
    /// </summary>
    public class AggregateRootExceptionDecorator
    {
        /// <summary>
        /// Gets the exception.
        /// </summary>
        public AggregateRootException Exception { get; private set; }

        /// <summary>
        /// Creates a new instance for modifying <paramref name="exception"/>.
        /// </summary>
        /// <param name="exception">An instance of aggregate root exception.</param>
        public AggregateRootExceptionDecorator(AggregateRootException exception)
        {
            Ensure.NotNull(exception, "exception");
            Exception = exception;
        }

        /// <summary>
        /// Sets an aggregate root key to <paramref name="key"/>.
        /// </summary>
        /// <param name="key">A key of the aggregate root.</param>
        /// <returns>Self (for fluency).</returns>
        public AggregateRootExceptionDecorator SetKey(IKey key)
        {
            Ensure.NotNull(key, "key");
            Exception.Key = key;
            return this;
        }

        /// <summary>
        /// Sets a command key to <paramref name="commandKey"/>.
        /// </summary>
        /// <param name="commandKey">A key of the command that caused the exception.</param>
        /// <returns>Self (for fluency).</returns>
        public AggregateRootExceptionDecorator SetCommandKey(IKey commandKey)
        {
            Ensure.NotNull(commandKey, "commandKey");
            Exception.CommandKey = commandKey;
            return this;
        }

        /// <summary>
        /// Sets a source command key to <paramref name="commandKey"/>.
        /// </summary>
        /// <param name="commandKey">A key of the original (first) command in the execution hain.</param>
        /// <returns>Self (for fluency).</returns>
        public AggregateRootExceptionDecorator SetSourceCommandKey(IKey commandKey)
        {
            Ensure.NotNull(commandKey, "commandKey");
            Exception.SourceCommandKey = commandKey;
            return this;
        }
    }
}
