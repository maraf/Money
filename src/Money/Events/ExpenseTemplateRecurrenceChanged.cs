using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events;

/// <summary>
/// An event raised when a recurrence of the expense template has changed.
/// </summary>
public class ExpenseTemplateRecurrenceChanged : UserEvent
{
    /// <summary>
    /// Gets a recurrence period.
    /// </summary>
    public RecurrencePeriod Period { get; private set; }

    /// <summary>
    /// Gets a day in period when the recurrence should happen.
    /// </summary>
    public int DayInPeriod { get; private set; }

    internal ExpenseTemplateRecurrenceChanged(RecurrencePeriod period, int dayInPeriod)
    {
        Period = period;
        DayInPeriod = dayInPeriod;
    }
}
