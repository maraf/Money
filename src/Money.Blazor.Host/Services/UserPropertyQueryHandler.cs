using Money.Models.Queries;
using Money.Queries;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    internal class UserPropertyQueryHandler : HttpQueryDispatcher.IMiddleware
    {
        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next)
        {
            if (query is GetDateFormatProperty)
            {
                var value = await dispatcher.QueryAsync(new FindUserProperty("DateFormat")) ?? CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
                return value;
            }
            else if (query is GetPriceDecimalDigitsProperty)
            {
                var value = IntPropertyValue(await dispatcher.QueryAsync(new FindUserProperty("PriceDecimalDigits")), 2);
                return value;
            }

            return await next(query);
        }

        private int IntPropertyValue(string value, int defaultValue)
        {
            if (Int32.TryParse(value, out int intValue))
                return intValue;

            return defaultValue;
        }
    }
}
