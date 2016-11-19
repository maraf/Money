using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Exceptions.Handlers
{
    /// <summary>
    /// The handler for exceptions raised during execution.
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Processes <paramref name="exception"/> raised during execution.
        /// </summary>
        /// <param name="exception">The exception to process.</param>
        void Handle(Exception exception);
    }

    /// <summary>
    /// The handler for exceptions of type <typeparamref name="T" /> raised during execution.
    /// </summary>
    /// <typeparam name="T">The type of the exception to handle.</typeparam>
    public interface IExceptionHandler<T>
        where T : Exception
    {
        /// <summary>
        /// Processes <paramref name="exception"/> raised during execution.
        /// </summary>
        /// <param name="exception">The exception to process.</param>
        void Handle(T exception);
    }
}
