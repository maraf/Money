using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Queries.Handlers
{
    /// <summary>
    /// Handler for query of type <typeparamref name="TQuery"/>.
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Should process <paramref name="query"/> and provide result of type <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <returns>Result to <paramref name="query"/>.</returns>
        Task<TResult> HandleAsync(TQuery query);
    }
}
