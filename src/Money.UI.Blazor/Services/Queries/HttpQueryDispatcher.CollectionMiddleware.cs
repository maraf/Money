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
            private Next next;

            public CollectionMiddleware(IEnumerable<IMiddleware> collection)
            {
                Ensure.NotNull(collection, "collection");
                enumerator = collection.GetEnumerator();
            }

            public Task<object> ExecuteAsync(object query, Next next)
            {
                this.next = next;
                return ExecuteNextAsync(query);
            }

            private Task<object> ExecuteNextAsync(object query)
            {
                if (enumerator.MoveNext())
                    return ExecuteCurrentAsync(query);

                return next(query);
            }

            private async Task<object> ExecuteCurrentAsync(object query)
            {
                object output = await enumerator.Current.ExecuteAsync(query, ExecuteNextAsync);
                return output;
            }
        }
    }
}
