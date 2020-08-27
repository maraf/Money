using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Queries
{
    partial class HttpQueryDispatcher
    {
        private class CollectionMiddleware : IMiddleware
        {
            private readonly IEnumerator<IMiddleware> enumerator;
            private HttpQueryDispatcher dispatcher;
            private Next next;

            public CollectionMiddleware(IEnumerable<IMiddleware> collection)
            {
                Ensure.NotNull(collection, "collection");
                enumerator = collection.GetEnumerator();
            }

            public Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, Next next)
            {
                this.next = next;
                this.dispatcher = dispatcher;
                return ExecuteNextAsync(query);
            }

            private Task<object> ExecuteNextAsync(object query)
            {
                if (enumerator.MoveNext())
                    return ExecuteCurrentAsync(query, dispatcher);

                return next(query);
            }

            private Task<object> ExecuteCurrentAsync(object query, HttpQueryDispatcher dispatcher)
            {
                return enumerator.Current.ExecuteAsync(query, dispatcher, ExecuteNextAsync);
            }
        }
    }
}
