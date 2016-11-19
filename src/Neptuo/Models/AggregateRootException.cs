using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models
{
    /// <summary>
    /// Base exception for aggregate root errors.
    /// </summary>
    public class AggregateRootException : Exception
    {
        /// <summary>
        /// The key of the aggregate root.
        /// </summary>
        /// <remarks>
        /// For exceptions thrown the correct way, contains a key of the aggregate root or empty key when creating new aggregate.
        /// If derived class sets value, the infrastructure doesn't overwrite it.
        /// </remarks>
        public IKey Key { get; protected internal set; }

        /// <summary>
        /// The key of the command that originated the operation.
        /// When the operation is composed from multiple commands, this property contains 
        /// the key of the first command (typically raised by the user).
        /// </summary>
        /// <remarks>
        /// For exceptions thrown the correct way, contains a key of the commnad.
        /// If derived class sets value, the infrastructure doesn't overwrite it.
        /// </remarks>
        public IKey SourceCommandKey { get; protected internal set; }

        /// <summary>
        /// The key of the command that raised the operation.
        /// </summary>
        /// <remarks>
        /// For exceptions thrown the correct way, contains a key of the commnad.
        /// If derived class sets value, the infrastructure doesn't overwrite it.
        /// </remarks>
        public IKey CommandKey { get; protected internal set; }

        /// <summary>
        /// Returns the key of the first command or <c>null</c>.
        /// If the <see cref="SourceCommandKey"/> is not <c>null</c>, returns it;
        /// otherwise returns <see cref="CommandKey"/> or <c>null</c>.
        /// </summary>
        /// <returns>The key of the first command or <c>null</c>.</returns>
        public IKey FindOriginalCommandKey()
        {
            if (SourceCommandKey == null || SourceCommandKey.IsEmpty)
                return CommandKey;

            return SourceCommandKey;
        }

        /// <summary>
        /// Creates new empty instance.
        /// </summary>
        public AggregateRootException() 
        { }

        /// <summary>
        /// Creates new instance with text <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The text description.</param>
        public AggregateRootException(string message) 
            : base(message) 
        { }

        /// <summary>
        /// Create new instance with text <paramref name="message"/> and <paramref name="inner"/> exception.
        /// </summary>
        /// <param name="message">The text description.</param>
        /// <param name="inner">The inner exception.</param>
        public AggregateRootException(string message, Exception inner) 
            : base(message, inner) 
        { }
    }
}
