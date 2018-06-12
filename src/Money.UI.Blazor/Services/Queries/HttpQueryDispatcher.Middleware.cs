using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Queries
{
    partial class HttpQueryDispatcher
    {
        public abstract class Middleware<TQuery, TOutput> : IMiddleware
            where TQuery : IQuery<TOutput>
        {
            public async Task<object> ExecuteAsync(object query, Next next)
            {
                if (query is TQuery typed)
                {
                    Console.WriteLine($"Typed: {typed.GetType().FullName}");
                    object output = await ExecuteAsync(typed, next);
                    Console.WriteLine($"Typed: {output}");
                    return output;
                }

                Console.WriteLine($"Typed: Next {next.Target.GetType().FullName}");
                return await next(query);
            }

            protected abstract Task<TOutput> ExecuteAsync(TQuery query, Next next);
        }
    }
}
