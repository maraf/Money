
using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;

namespace Money.Commands;

/// <summary>
/// Sets the recurrence of the expense template with <see cref="ExpenseTemplateKey"/>.
/// </summary>
public class SetExpenseTemplateYearlyRecurrence : Command, IExpenseTemplateCommand, IAggregateRootCommand
{
    /// <summary>
    /// Gets a key of the expense template to modify.
    /// </summary>
    public IKey ExpenseTemplateKey { get; private set; }

    IKey IAggregateRootCommand.AggregateKey => ExpenseTemplateKey;

    /// <summary>
    /// Gets a number of periods when the recurrence should happen.
    /// </summary>
    public int MonthInPeriod { get; private set; }

    /// <summary>
    /// Gets a day in period when the recurrence should happen.
    /// </summary>
    public int DayInPeriod { get; private set; }

    public SetExpenseTemplateYearlyRecurrence(IKey expenseTemplateKey, int monthInPeriod, int dayInPeriod)
    {
        Ensure.Condition.NotEmptyKey(expenseTemplateKey);
        Ensure.Positive(monthInPeriod, "monthInPeriod");
        Ensure.Positive(dayInPeriod, "dayInPeriod");
        ExpenseTemplateKey = expenseTemplateKey;
        MonthInPeriod = monthInPeriod;
        DayInPeriod = dayInPeriod;
    }
}
