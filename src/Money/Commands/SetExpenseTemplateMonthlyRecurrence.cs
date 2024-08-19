using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands;

/// <summary>
/// Sets the recurrence of the expense template with <see cref="ExpenseTemplateKey"/>.
/// </summary>
public class SetExpenseTemplateMonthlyRecurrence : Command, IExpenseTemplateCommand, IAggregateRootCommand
{
    /// <summary>
    /// Gets a key of the expense template to modify.
    /// </summary>
    public IKey ExpenseTemplateKey { get; private set; }

    IKey IAggregateRootCommand.AggregateKey => ExpenseTemplateKey;

    /// <summary>
    /// Gets a day in period when the recurrence should happen.
    /// </summary>
    public int DayInPeriod { get; private set; }

    public SetExpenseTemplateMonthlyRecurrence(IKey expenseTemplateKey, int dayInPeriod)
    {
        Ensure.Condition.NotEmptyKey(expenseTemplateKey);
        Ensure.Positive(dayInPeriod, "dayInPeriod");
        ExpenseTemplateKey = expenseTemplateKey;
        DayInPeriod = dayInPeriod;
    }
}
