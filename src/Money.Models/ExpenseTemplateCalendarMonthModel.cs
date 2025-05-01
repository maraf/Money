using Neptuo;
using Neptuo.Models.Keys;

namespace Money.Models;

public class ExpenseTemplateCalendarMonthModel : MonthModel
{
    public override int Year => base.Year;
    public override int Month => base.Month;
    public IKey ExpenseTemplateKey { get; private set; }

    public Price ExpenseTotal { get; set; }
    public int ExpenseCount { get; set; }

    public ExpenseTemplateCalendarMonthModel(int year, int month, IKey expenseTemplateKey, Price expenseTotal, int expenseCount)
        : base(year, month)
    {
        Ensure.Condition.NotEmptyKey(expenseTemplateKey);
        Ensure.NotNull(expenseTotal, "expenseTotal");
        Ensure.PositiveOrZero(expenseCount, "expenseCount");
        ExpenseTemplateKey = expenseTemplateKey;
        ExpenseTotal = expenseTotal;
        ExpenseCount = expenseCount;
    }
}