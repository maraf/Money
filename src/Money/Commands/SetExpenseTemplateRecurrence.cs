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
public class SetExpenseTemplateRecurrence : Command, IExpenseTemplateCommand
{
    /// <summary>
    /// Gets a key of the expense template to modify.
    /// </summary>
    public IKey ExpenseTemplateKey { get; private set; }

    /// <summary>
    /// Gets a recurrence period.
    /// </summary>
    public RecurrencePeriod Period { get; private set; }

    /// <summary>
    /// Gets a day in period when the recurrence should happen.
    /// </summary>
    public int DayInPeriod { get; private set; }

    public SetExpenseTemplateRecurrence(IKey expenseTemplateKey, RecurrencePeriod period, int dayInPeriod)
    {
        Ensure.Condition.NotEmptyKey(expenseTemplateKey);
        Ensure.Positive(dayInPeriod, "dayInPeriod");
        ExpenseTemplateKey = expenseTemplateKey;
        Period = period;
        DayInPeriod = dayInPeriod;
    }
}
