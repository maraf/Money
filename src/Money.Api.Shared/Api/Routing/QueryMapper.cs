using Money.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Api.Routing
{
    public class QueryMapper : TypeMapper
    {
        public QueryMapper()
        {
            Add<ListAllCategory>("category-list");
            Add<GetCategoryName>("category-name");
            Add<GetCategoryNameDescription>("category-name-description");
            Add<GetCategoryColor>("category-color");
            Add<GetCategoryIcon>("category-icon");

            Add<ListAllCurrency>("currency-list");
            Add<GetCurrencyDefault>("currency-default");
            Add<FindCurrencyDefault>("currency-default-nullable");
            Add<GetCurrencySymbol>("currency-symbol");
            Add<ListTargetCurrencyExchangeRates>("currency-exchangerates");

            Add<GetTotalMonthOutcome>("month-outcome-total");
            Add<ListMonthCategoryWithOutcome>("month-categories-with-outcome");
            Add<ListMonthOutcomeFromCategory>("month-outcome-from-category");
            Add<ListMonthWithOutcome>("months-with-outcome");

            Add<ListAllExpenseTemplate>("expense-templates");

            Add<GetTotalMonthIncome>("month-income-total");
            Add<ListMonthIncome>("month-incomes");

            Add<GetTotalYearOutcome>("year-outcome-total");
            Add<ListYearCategoryWithOutcome>("year-categories-with-outcome");
            Add<ListYearOutcomeFromCategory>("year-outcome-from-category");
            Add<ListYearWithOutcome>("years-with-outcome");

            Add<ListMonthOutcomesForCategory>("month-outcomes-for-category");
            Add<ListYearOutcomesForCategory>("year-outcomes-for-category");

            Add<ListMonthBalance>("month-balances");

            Add<SearchOutcomes>("search-outcomes");

            Add<GetProfile>("user-profile");
            Add<ListUserProperty>("user-properties");
            Add<FindUserProperty>("user-property");
        }
    }
}
