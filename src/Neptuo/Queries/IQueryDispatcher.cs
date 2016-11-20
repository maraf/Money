using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Queries
{
    /// <summary>
    /// Dispatcher for queries.
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        /// Dispatches <paramref name="query"/> for providing result.
        /// </summary>
        /// <typeparam name="TOutput">Type of result.</typeparam>
        /// <param name="query">Query parameters.</param>
        /// <returns>Result to <paramref name="query"/>.</returns>
        Task<TOutput> QueryAsync<TOutput>(IQuery<TOutput> query);
    }
}
