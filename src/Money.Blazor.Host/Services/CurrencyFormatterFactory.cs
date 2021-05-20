using Money.Models.Queries;
using Neptuo;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class CurrencyFormatterFactory
    {
        private readonly IQueryDispatcher queries;

        public CurrencyFormatterFactory(IQueryDispatcher queries)
        {
            Ensure.NotNull(queries, "queries");
            this.queries = queries;
        }

        public async Task<CurrencyFormatter> CreateAsync()
        {
            var currencies = await queries.QueryAsync(new ListAllCurrency());
            var decimalDigits = IntPropertyValue(await queries.QueryAsync(new FindUserProperty("PriceDecimalDigits")), 2);

            return new CurrencyFormatter(currencies, decimalDigits);
        }

        private int IntPropertyValue(string value, int defaultValue)
        {
            if (Int32.TryParse(value, out int intValue))
                return intValue;

            return defaultValue;
        }
    }
}
