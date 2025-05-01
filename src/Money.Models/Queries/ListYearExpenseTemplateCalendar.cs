using System.Collections.Generic;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Queries;

namespace Money.Models.Queries;

public class ListYearExpenseTemplateCalendar : UserQuery, IQuery<List<ExpenseTemplateCalendarMonthModel>>
{
    public YearModel Year { get; private set; }
    public IKey ExpenseTemplateKey { get; private set; }

    public ListYearExpenseTemplateCalendar(YearModel year, IKey expenseTemplateKey)
    {
        Ensure.NotNull(year, "year");
        Ensure.Condition.NotEmptyKey(expenseTemplateKey);

        Year = year;
        ExpenseTemplateKey = expenseTemplateKey;
    }
}