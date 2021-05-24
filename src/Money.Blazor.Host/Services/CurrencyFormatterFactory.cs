using Money.Models.Queries;
using Money.Queries;
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
            var decimalDigits = await queries.QueryAsync(new GetPriceDecimalDigitsProperty());

            return new CurrencyFormatter(currencies, decimalDigits);
        }
    }
}
