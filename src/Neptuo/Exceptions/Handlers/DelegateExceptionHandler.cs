using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Exceptions.Handlers
{
    /// <summary>
    /// The factory class for exception handlers from actions.
    /// </summary>
    public static class DelegateExceptionHandler
    {
        /// <summary>
        /// Creates new instance using <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">Delegate for handling exceptions.</param>
        public static IExceptionHandler FromAction(Action<Exception> handler)
        {
            Ensure.NotNull(handler, "action");
            return new ExceptionHandler(handler);
        }

        /// <summary>
        /// Creates new instance using <paramref name="handler"/>.
        /// </summary>
        /// <typeparam name="T">The of the exception to handle.</typeparam>
        /// <param name="handler">Delegate for handling exceptions.</param>
        public static IExceptionHandler<T> FromAction<T>(Action<T> handler)
            where T : Exception
        {
            Ensure.NotNull(handler, "action");
            return new ExceptionHandler<T>(handler);
        }

        private class ExceptionHandler : ExceptionHandler<Exception>, IExceptionHandler
        {
            public ExceptionHandler(Action<Exception> handler)
                : base(handler)
            { }
        }

        private class ExceptionHandler<T> : IExceptionHandler<T>
            where T : Exception
        {
            private readonly Action<T> handler;

            public ExceptionHandler(Action<T> handler)
            {
                this.handler = handler;
            }

            public void Handle(T exception)
            {
                handler(exception);
            }
        }
    }
}
