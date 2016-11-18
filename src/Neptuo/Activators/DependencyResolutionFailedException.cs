using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Exception that should by thrown by all implementations of <see cref="IDependencyProvider"/> when instance can't provided.
    /// </summary>
    public class DependencyResolutionFailedException : DependencyException
    {
        /// <summary>
        /// Creates empty instance.
        /// </summary>
        public DependencyResolutionFailedException() 
        { }

        /// <summary>
        /// Creates instance with text message.
        /// </summary>
        /// <param name="message">Text description of the occurred error.</param>
        public DependencyResolutionFailedException(string message) 
            : base(message) 
        { }

        /// <summary>
        /// Creates instance with text message and inner exception.
        /// </summary>
        /// <param name="message">Text description of the occurred error.</param>
        /// <param name="inner">Source exception that caused problem.</param>
        public DependencyResolutionFailedException(string message, Exception inner) 
            : base(message, inner) 
        { }
    }
}
