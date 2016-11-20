using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Queries.Internals
{
    /// <summary>
    /// Basic definition of query handler.
    /// </summary>
    internal class DefaultQueryHandlerDefinition
    {
        /// <summary>
        /// Query handler.
        /// Should never be <c>null</c>.
        /// </summary>
        public object QueryHandler { get; set; }

        public DefaultQueryHandlerDefinition(object queryHandler)
        {
            QueryHandler = queryHandler;
        }
    }

    /// <summary>
    /// Output typed definition of query handler.
    /// Used when handling query from <see cref="IQueryDispatcher"/> for <see cref="IQuery{TOutput}"/>.
    /// </summary>
    /// <typeparam name="TOutput">Type of query output.</typeparam>
    internal class DefaultQueryHandlerDefinition<TOutput> : DefaultQueryHandlerDefinition
    {
        public Func<object, Task<TOutput>> HandleMethod { get; set; }

        public DefaultQueryHandlerDefinition(object queryHandler, Func<object, Task<TOutput>> handleMethod)
            : base(queryHandler)
        {
            HandleMethod = handleMethod;
        }

        public Task<TOutput> HandleAsync(IQuery<TOutput> query)
        {
            return HandleMethod(query);
        }
    }
}
