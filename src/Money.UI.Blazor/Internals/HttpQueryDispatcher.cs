using Money.Models.Api;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Formatters;
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

        public HttpQueryDispatcher(ApiClient api, FormatterContainer formatters)
        {
            Ensure.NotNull(api, "api");
            Ensure.NotNull(formatters, "formatters");
            this.api = api;
            this.formatters = formatters;
        }

        public async Task<TOutput> QueryAsync<TOutput>(IQuery<TOutput> query)
        {
            if (query is GetCategoryName getCategoryName)
            {
                // TODO: Nasty composite formatter workaround.
                var categories = await QueryInternalAsync(new ListAllCategory());
                var category = categories.First(c => c.Key.Equals(getCategoryName.CategoryKey));
                return (TOutput)(object)category.Name;
            }

            return await QueryInternalAsync(query);
        }

        private async Task<TOutput> QueryInternalAsync<TOutput>(IQuery<TOutput> query)
        {
            string payload = formatters.Query.Serialize(query);
            string type = query.GetType().AssemblyQualifiedName;

            Response response = await api.QueryAsync(new Request()
            {
                Payload = payload,
                Type = type
            });

            Console.WriteLine($"Response Type: {response.type}");
            Console.WriteLine($"Response Payload: {response.payload}");
            if (!string.IsNullOrEmpty(response.payload))
            {
                TOutput output = formatters.Query.Deserialize<TOutput>(response.payload);
                Console.WriteLine($"HQD: Output success: {output != null}.");
                return output;
            }

            Console.WriteLine("HQD: Fallback to default value.");
            return Activator.CreateInstance<TOutput>();
        }
    }
}
