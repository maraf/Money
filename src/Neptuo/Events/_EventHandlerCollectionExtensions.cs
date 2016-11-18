using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Events.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Events
{
    /// <summary>
    /// Common extensions for <see cref="IEventHandlerCollection"/>.
    /// </summary>
    public static class _EventHandlerCollectionExtensions
    {
        /// <summary>
        /// Registers <paramref name="handler"/> to handle events for all implemented interfaces <see cref="IEventHandler{T}"/>.
        /// </summary>
        /// <param name="collection">The collection of events handlers.</param>
        /// <param name="handler">The event handler.</param>
        /// <returns><paramref name="collection"/>.</returns>
        public static IEventHandlerCollection AddAll(this IEventHandlerCollection collection, object handler)
        {
            Ensure.NotNull(collection, "collection");
            Ensure.NotNull(handler, "handler");
            foreach (Type interfaceType in handler.GetType().GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && typeof(IEventHandler<>) == interfaceType.GetGenericTypeDefinition())
                {
                    MethodInfo addMethod = collection.GetType().GetMethod(Constant.EventHandlerCollectionAddMethodName).MakeGenericMethod(interfaceType.GetGenericArguments());
                    addMethod.Invoke(collection, new object[] { handler });
                }
            }

            return collection;
        }
    }
}
