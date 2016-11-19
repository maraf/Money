using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// The base exception for formatters
    /// </summary>
    public class FormatterException : Exception
    {
        /// <summary>
        /// Creates new empty instance.
        /// </summary>
        protected FormatterException()
        { }

        /// <summary>
        /// Creates new instance with the <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The text description of the problem.</param>
        public FormatterException(string message)
            : base(message)
        { }

        /// <summary>
        /// Creates new instance with the <paramref name="message"/> and <paramref name="inner"/> exception.
        /// </summary>
        /// <param name="message">The text description of the problem.</param>
        /// <param name="inner">The inner cause of the exceptional state.</param>
        public FormatterException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
