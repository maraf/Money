using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Formatters.Metadata;

namespace Money.Events;

/// <summary>
/// An event raised when a recurrence of the expense template has changed.
/// </summary>
public class ExpenseTemplateRecurrenceChanged : UserEvent
{
    [CompositeVersion]
    public int CompositeVersion { get; set; }

    /// <summary>
    /// Gets a recurrence period.
    /// </summary>
    [CompositeProperty(1)]
    public RecurrencePeriod Period { get; private set; }

    /// <summary>
    /// Gets a number of periods when the recurrence should happen for <see cref="RecurrencePeriod.XMonths">.
    /// </summary>
    [CompositeProperty(2, Version = 2)]
    public int? EveryXPeriods { get; private set; }

    /// <summary>
    /// Gets a month in period when the recurrence should happen for <see cref="RecurrencePeriod.Yearly">.
    /// </summary>
    [CompositeProperty(3, Version = 2)]
    public int? MonthInPeriod { get; private set; }

    /// <summary>
    /// Gets a day in period when the recurrence should happen for <see cref="RecurrencePeriod.Monthly"> and <see cref="RecurrencePeriod.Yearly">.
    /// </summary>
    [CompositeProperty(2)]
    [CompositeProperty(4, Version = 2)]
    public int? DayInPeriod { get; private set; }

    /// <summary>
    /// Gets a due date when the recurrence should happen for <see cref="RecurrencePeriod.Single">.
    /// </summary>
    [CompositeProperty(2)]
    [CompositeProperty(5, Version = 2)]
    public DateTime? DueDate { get; private set; }

    [CompositeConstructor(Version = 1)]
    internal ExpenseTemplateRecurrenceChanged(RecurrencePeriod period, int? dayInPeriod = null, DateTime? dueDate = null)
    {
        Period = period;
        DayInPeriod = dayInPeriod;
        DueDate = dueDate;
        
        CompositeVersion = 1;
    }

    [CompositeConstructor(Version = 2)]
    internal ExpenseTemplateRecurrenceChanged(RecurrencePeriod period, int? everyXPeriods = null, int? monthInPeriod = null, int? dayInPeriod = null, DateTime? dueDate = null)
        : this(period, dayInPeriod, dueDate)
    {
        EveryXPeriods = everyXPeriods;
        MonthInPeriod = monthInPeriod;

        CompositeVersion = 2;
    }
}
