using Neptuo.Queries.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Queries
{
    /// <summary>
    /// Collection of registered query handlers.
    /// </summary>
    public interface IQueryHandlerCollection
    {
        /// <summary>
        /// Registers <paramref name="handler"/> to handle queries of type <typeparamref name="TQuery"/>.
        /// </summary>
        /// <typeparam name="TQuery">Type of query.</typeparam>
        /// <typeparam name="TOutput">Type of result.</typeparam>
        /// <param name="handler">Handler for queries of type <typeparamref name="TQuery"/>.</param>
        /// <returns>Self (for fluency).</returns>
        IQueryHandlerCollection Add<TQuery, TOutput>(IQueryHandler<TQuery, TOutput> handler)
             where TQuery : IQuery<TOutput>;

        /// <summary>
        /// Tries to find query handler for query of type <typeparamref name="TQuery"/>.
        /// </summary>
        /// <typeparam name="TQuery">Type of query.</typeparam>
        /// <typeparam name="TOutput">Type of result.</typeparam>
        /// <param name="handler">Handler for queries of type <typeparamref name="TQuery"/>.</param>
        /// <returns><c>true</c> if such a handler is registered; <c>false</c> otherwise.</returns>
        bool TryGet<TQuery, TOutput>(out IQueryHandler<TQuery, TOutput> handler)
             where TQuery : IQuery<TOutput>;
    }
}
