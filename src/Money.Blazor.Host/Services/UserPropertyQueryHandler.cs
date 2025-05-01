using Money.Models;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Queries;
using Money.Pages;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Money.Components;

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
            else if (query is GetSummarySortProperty)
            {
                return await GetSortDescriptorAsync<SummarySortType>(dispatcher, "SummarySort", "ByCategory-Ascending");
            }
            else if (query is GetExpenseOverviewSortProperty) 
            {
                return await GetSortDescriptorAsync<OutcomeOverviewSortType>(dispatcher, "ExpenseOverviewSort", "ByWhen-Descending");
            }
            else if (query is GetSearchSortProperty) 
            {
                return await GetSortDescriptorAsync<OutcomeOverviewSortType>(dispatcher, "SearchSort", "ByWhen-Descending");
            }
            else if (query is GetBalanceDisplayProperty) 
            {
                return await GetEnumAsync<BalanceDisplayType>(dispatcher, "BalanceDisplay", "Total");
            }
            else if (query is GetSummaryDisplayProperty) 
            {
                return await GetEnumAsync<SummaryDisplayType>(dispatcher, "SummaryDisplay", "Total");
            }
            else if (query is GetExpenseTemplateSortProperty) 
            {
                return await GetSortDescriptorAsync<ExpenseTemplateSortType>(dispatcher, "ExpenseTemplateSort", "ByDescription-Ascending");
            }
            else if (query is GetExpenseTemplateCalendarDisplayProperty) 
            {
                return await GetEnumAsync<ExpenseTemplateCalendarDisplayType>(dispatcher, "ExpenseTemplateCalendarDisplay", "Check");
            }
            else if (query is GetExpenseCreateDialogTypeProperty) 
            {
                return await GetEnumAsync<ExpenseCreateDialogType>(dispatcher, GetExpenseCreateDialogTypeProperty.PropertyKey, "Standard");
            }
            else if (query is GetThemeTypeProperty) 
            {
                return await GetEnumAsync<ThemeType>(dispatcher, GetThemeTypeProperty.PropertyKey, "Light");
            }

            return await next(query);
        }

        private async Task<SortDescriptor<T>> GetSortDescriptorAsync<T>(HttpQueryDispatcher dispatcher, string propertyName, string defaultValue)
            where T : struct
        {
            var value = await dispatcher.QueryAsync(new FindUserProperty(propertyName));
            if (String.IsNullOrEmpty(value))
                value = defaultValue;

            string[] parts = value.Split('-');

            var result = new SortDescriptor<T>(
                Enum.Parse<T>(parts[0], true),
                Enum.Parse<SortDirection>(parts[1], true)
            );

            return result;
        }

        private async Task<T> GetEnumAsync<T>(HttpQueryDispatcher dispatcher, string propertyName, string defaultValue)
            where T : struct
        {
            var value = await dispatcher.QueryAsync(new FindUserProperty(propertyName));
            if (String.IsNullOrEmpty(value))
                value = defaultValue;

            return Enum.Parse<T>(value, true);
        }

        private int IntPropertyValue(string value, int defaultValue)
        {
            if (Int32.TryParse(value, out int intValue))
                return intValue;

            return defaultValue;
        }
    }
}
