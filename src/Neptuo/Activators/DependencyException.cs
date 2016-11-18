using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Generic base exception for <see cref="IDependencyContainer"/> and <see cref="IDependencyProvider"/>.
    /// </summary>
    public abstract class DependencyException : Exception
    {
        /// <summary>
        /// Creates empty instance.
        /// </summary>
        public DependencyException() 
        { }

        /// <summary>
        /// Creates instance with text message.
        /// </summary>
        /// <param name="message">Text description of the occurred error.</param>
        public DependencyException(string message) 
            : base(message) 
        { }

        /// <summary>
        /// Creates instance with text message and inner exception.
        /// </summary>
        /// <param name="message">Text description of the occurred error.</param>
        /// <param name="inner">Source exception that caused problem.</param>
        public DependencyException(string message, Exception inner) 
            : base(message, inner) 
        { }
    }
}
