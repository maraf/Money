using Neptuo.Queries.Handlers;
using Neptuo.Queries.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Queries
{
    /// <summary>
    /// Common extensions for <see cref="IQueryHandlerCollection"/>.
    /// </summary>
    public static class _QueryHandlerCollectionExtensions
    {
        /// <summary>
        /// Registers <paramref name="handler"/> to handle queries for all implemented interfaces <see cref="IQuerHandler{TQuery, TResult}"/>.
        /// </summary>
        /// <param name="collection">Collection of query handlers.</param>
        /// <param name="handler">Query handler.</param>
        /// <returns><paramref name="collection"/>.</returns>
        public static IQueryHandlerCollection AddAll(this IQueryHandlerCollection collection, object handler)
        {
            Ensure.NotNull(collection, "collection");
            Ensure.NotNull(handler, "handler");
            foreach (Type interfaceType in handler.GetType().GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && typeof(IQueryHandler<,>) == interfaceType.GetGenericTypeDefinition())
                {
                    MethodInfo addMethod = collection.GetType().GetMethod(Constant.QueryHandlerCollectionAddMethodName).MakeGenericMethod(interfaceType.GetGenericArguments());
                    addMethod.Invoke(collection, new object[] { handler });
                }
            }

            return collection;
        }
    }
}
