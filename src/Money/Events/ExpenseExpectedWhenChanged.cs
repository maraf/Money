using System;

namespace Money.Events;

/// <summary>
/// An event raised when an expected date when the outcome occured has changed.
/// </summary>
public class ExpenseExpectedWhenChanged(DateTime when) : UserEvent
{
    /// <summary>
    /// Get a new date when the outcome was expected to occur.
    /// </summary>
    public DateTime When { get; private set; } = when;
}
