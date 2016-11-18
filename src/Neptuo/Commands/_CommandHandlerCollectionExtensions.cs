using Neptuo.Commands;
using Neptuo.Commands.Handlers;
using Neptuo.Commands.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    /// <summary>
    /// Common extensions for <see cref="ICommandHandlerCollection"/>.
    /// </summary>
    public static class _CommandHandlerCollectionExtensions
    {
        /// <summary>
        /// Registers <paramref name="handler"/> to handle commands for all implemented interfaces <see cref="ICommandHandler{T}"/>.
        /// </summary>
        /// <param name="collection">The collection of command handlers.</param>
        /// <param name="handler">The commmand handler.</param>
        /// <returns><paramref name="collection"/>.</returns>
        public static ICommandHandlerCollection AddAll(this ICommandHandlerCollection collection, object handler)
        {
            Ensure.NotNull(collection, "collection");
            Ensure.NotNull(handler, "handler");
            foreach (Type interfaceType in handler.GetType().GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && typeof(ICommandHandler<>) == interfaceType.GetGenericTypeDefinition())
                {
                    MethodInfo addMethod = collection.GetType().GetMethod(Constant.CommandHandlerCollectionAddMethodName).MakeGenericMethod(interfaceType.GetGenericArguments());
                    addMethod.Invoke(collection, new object[] { handler });
                }
            }

            return collection;
        }
    }
}
