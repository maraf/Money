using Neptuo.Queries.Handlers;
using Neptuo.Queries.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Queries
{
    /// <summary>
    /// Default implementation of <see cref="IQueryDispatcher"/> and <see cref="IQueryHandlerCollection"/>.
    /// When handling query and query handler is missing, exception is thrown.
    /// </summary>
    public class DefaultQueryDispatcher : IQueryHandlerCollection, IQueryDispatcher
    {
        private readonly Dictionary<Type, DefaultQueryHandlerDefinition> storage = new Dictionary<Type, DefaultQueryHandlerDefinition>();

        public IQueryHandlerCollection Add<TQuery, TOutput>(IQueryHandler<TQuery, TOutput> handler) 
            where TQuery : IQuery<TOutput>
        {
            Ensure.NotNull(handler, "handler");
            DefaultQueryHandlerDefinition<TOutput> definition = new DefaultQueryHandlerDefinition<TOutput>(handler, query => handler.HandleAsync((TQuery)query));
            storage[typeof(TQuery)] = definition;
            return this;
        }

        public bool TryGet<TQuery, TOutput>(out IQueryHandler<TQuery, TOutput> handler)
            where TQuery : IQuery<TOutput>
        {
            DefaultQueryHandlerDefinition definition;
            if(storage.TryGetValue(typeof(TQuery), out definition))
            {
                handler = (IQueryHandler<TQuery, TOutput>)definition.QueryHandler;
                return true;
            }

            handler = null;
            return false;
        }

        public Task<TOutput> QueryAsync<TOutput>(IQuery<TOutput> query)
        {
            Ensure.NotNull(query, "query");
            DefaultQueryHandlerDefinition definition;

            Type queryType = query.GetType();
            if (storage.TryGetValue(queryType, out definition))
            {
                DefaultQueryHandlerDefinition<TOutput> target = (DefaultQueryHandlerDefinition<TOutput>)definition;
                return target.HandleAsync(query);
            }

            throw Ensure.Exception.ArgumentOutOfRange("query", "There isn't query handler for query of type '{0}'.", queryType.FullName);
        }
    }
}
