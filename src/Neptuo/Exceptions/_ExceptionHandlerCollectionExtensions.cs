using Neptuo.Exceptions.Handlers;
using Neptuo.Exceptions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Exceptions
{
    /// <summary>
    /// The common extensions for the <see cref="IExceptionHandlerCollection"/>.
    /// </summary>
    public static class _ExceptionHandlerCollectionExtensions
    {
        /// <summary>
        /// Adds <paramref name="handler"/> to handle exceptions of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the exception to handle by the <paramref name="handler"/>.</typeparam>
        /// <param name="collection">The collection to add handler to.</param>
        /// <param name="handler">The handler to add to the <paramref name="collection"/>.</param>
        /// <returns>Self (for fluency).</returns>
        public static IExceptionHandlerCollection Add<T>(this IExceptionHandlerCollection collection, IExceptionHandler<T> handler)
            where T : Exception
        {
            Ensure.NotNull(collection, "collection");
            return collection.Add(new ExceptionHandlerBuilder().Handler(handler));
        }

        /// <summary>
        /// Registers <paramref name="handler"/> to handle exceptions for all implemented interfaces <see cref="IExceptionHandler{T}"/>.
        /// </summary>
        /// <param name="collection">The collection of exception handlers.</param>
        /// <param name="handler">The exceptions handler.</param>
        /// <returns><paramref name="collection"/>.</returns>
        public static IExceptionHandlerCollection AddAll(this IExceptionHandlerCollection collection, object handler)
        {
            Ensure.NotNull(collection, "collection");
            Ensure.NotNull(handler, "handler");
            foreach (Type interfaceType in handler.GetType().GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && typeof(IExceptionHandler<>) == interfaceType.GetGenericTypeDefinition())
                {
                    MethodInfo addMethod = typeof(_ExceptionHandlerCollectionExtensions).GetMethod(Constant.ExceptionHandlerCollectionAddMethodName).MakeGenericMethod(interfaceType.GetGenericArguments());
                    addMethod.Invoke(collection, new object[] { collection, handler });
                }
            }

            return collection;
        }

        /// <summary>
        /// Enumerates all handlers in the <paramref name="collection"/> and passes each of them <paramref name="exception"/>.
        /// </summary>
        /// <param name="collection">The collection use handlers from.</param>
        /// <param name="exception">The exception to handle.</param>
        public static void Handle(this IExceptionHandlerCollection collection, Exception exception)
        {
            Ensure.NotNull(collection, "collection");
            Ensure.NotNull(exception, "exception");

            foreach (IExceptionHandler handler in collection.Enumerate())
                handler.Handle(exception);
        }
    }
}
