using Money.Api.Models;
using Money.Services;
using Neptuo.Formatters;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Queries
{
    internal partial class HttpQueryDispatcher : IQueryDispatcher
    {
        private readonly ApiClient api;
        private readonly FormatterContainer formatters;
        private readonly ILog log;
        private readonly IEnumerable<IMiddleware> middlewares;

        public HttpQueryDispatcher(ApiClient api, FormatterContainer formatters, ILogFactory logFactory, IEnumerable<IMiddleware> middlewares)
        {
            Ensure.NotNull(api, "api");
            Ensure.NotNull(formatters, "formatters");
            Ensure.NotNull(logFactory, "logFactory");
            Ensure.NotNull(middlewares, "middlewares");
            this.api = api;
            this.formatters = formatters;
            this.middlewares = middlewares;
            this.log = logFactory.Scope("HttpQueryDispatcher");
        }

        public async Task<TOutput> QueryAsync<TOutput>(IQuery<TOutput> query)
        {
            log.Debug($"Query input '{query.GetType().Name}' casting to '{typeof(IQuery<TOutput>).Name}' ('{typeof(TOutput).Name}').");

            CollectionMiddleware middleware = new CollectionMiddleware(middlewares);
            TOutput output = (TOutput)await middleware.ExecuteAsync(query, this, ExecuteRawAsync);
            return output;
        }

        private async Task<object> ExecuteRawAsync(object query)
        {
            var outputType = GetOutputType(query);

            if (log.IsDebugEnabled())
                log.Debug($"Query '{query.GetType()}' with output '{outputType}'.");

            string payload = formatters.Query.Serialize(query);
            Type type = query.GetType();
            log.Debug($"Request Type: '{type.AssemblyQualifiedName}'.");
            log.Debug($"Request Payload: '{payload}'.");

            Response response = await api.QueryAsync(type, payload);

            log.Debug($"Response Type: '{response.Type}'");
            log.Debug($"Response Payload: '{response.Payload}'");
            if (!string.IsNullOrEmpty(response.Payload))
            {
                if (response.ResponseType == ResponseType.Plain)
                {
                    object output = Converts.To(outputType, response.Payload);
                    log.Debug($"Output success (plain): '{output != null}'.");
                    return output;
                }
                else
                {
                    object output = formatters.Query.Deserialize(outputType, response.Payload);
                    log.Debug($"Output success (composite): '{output != null}'.");
                    return output;
                }
            }

            log.Debug("Fallback to null value.");
            return null;
        }

        private static Type GetOutputType(object query)
        {
            var queryType = query.GetType();
            foreach (var interfaceType in queryType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>))
                    return interfaceType.GetGenericArguments()[0];
            }

            throw Ensure.Exception.InvalidOperation($"Unnable to find query return type in '{queryType.FullName}'.");
        }
    }
}
