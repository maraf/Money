using Money.Models.Api;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Formatters;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    internal class HttpQueryDispatcher : IQueryDispatcher
    {
        private readonly ApiClient api;
        private readonly FormatterContainer formatters;
        private readonly ILog log;

        public HttpQueryDispatcher(ApiClient api, FormatterContainer formatters, ILogFactory logFactory)
        {
            Ensure.NotNull(api, "api");
            Ensure.NotNull(formatters, "formatters");
            Ensure.NotNull(logFactory, "logFactory");
            this.api = api;
            this.formatters = formatters;
            this.log = logFactory.Scope("HttpQueryDispatcher");
        }

        public async Task<TOutput> QueryAsync<TOutput>(IQuery<TOutput> query)
        {
            string payload = formatters.Query.Serialize(query);
            string type = query.GetType().AssemblyQualifiedName;
            log.Debug($"Request Type: '{type}'.");
            log.Debug($"Request Payload: '{payload}'.");

            Response response = await api.QueryAsync(new Request()
            {
                Payload = payload,
                Type = type
            });

            log.Debug($"Response Type: '{response.Type}'");
            log.Debug($"Response Payload: '{response.Payload}'");
            if (!string.IsNullOrEmpty(response.Payload))
            {
                if (response.ResponseType == ResponseType.Plain)
                {
                    TOutput output = (TOutput)Converts.To(typeof(TOutput), response.Payload);
                    log.Debug($"Output success (plain): '{output != null}'.");
                    return output;
                }
                else
                {
                    TOutput output = formatters.Query.Deserialize<TOutput>(response.Payload);
                    log.Debug($"Output success (composite): '{output != null}'.");
                    return output;
                }
            }

            log.Debug("Fallback to default value.");
            return Activator.CreateInstance<TOutput>();
        }
    }
}
