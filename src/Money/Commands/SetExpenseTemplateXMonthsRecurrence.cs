using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;

namespace Money.Commands;

/// <summary>
/// Sets the recurrence of the expense template with <see cref="ExpenseTemplateKey"/>.
/// </summary>
public class SetExpenseTemplateXMonthsRecurrence : Command, IExpenseTemplateCommand, IAggregateRootCommand
{
    /// <summary>
    /// Gets a key of the expense template to modify.
    /// </summary>
    public IKey ExpenseTemplateKey { get; private set; }

    IKey IAggregateRootCommand.AggregateKey => ExpenseTemplateKey;

    /// <summary>
    /// Gets a number of periods when the recurrence should happen.
    /// </summary>
    public int EveryXPeriods { get; private set; }

    /// <summary>
    /// Gets a day in period when the recurrence should happen.
    /// </summary>
    public int DayInPeriod { get; private set; }

    public SetExpenseTemplateXMonthsRecurrence(IKey expenseTemplateKey, int everyXPeriods, int dayInPeriod)
    {
        Ensure.Condition.NotEmptyKey(expenseTemplateKey);
        Ensure.Positive(everyXPeriods, "everyXPeriods");
        Ensure.Positive(dayInPeriod, "dayInPeriod");
        ExpenseTemplateKey = expenseTemplateKey;
        EveryXPeriods = everyXPeriods;
        DayInPeriod = dayInPeriod;
    }
}
