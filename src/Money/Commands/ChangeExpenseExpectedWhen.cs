using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using System;

namespace Money.Commands;

/// <summary>
/// Changes a <see cref="ExpectedWhen"/> of the expense with <see cref="ExpenseKey"/>.
/// </summary>
public class ChangeExpenseExpectedWhen : Command, IAggregateRootCommand
{
    /// <summary>
    /// Gets a key of the expense to modify.
    /// </summary>
    public IKey ExpenseKey { get; private set; }

    IKey IAggregateRootCommand.AggregateKey => ExpenseKey;

    /// <summary>
    /// Gets a date when the expense was expected to occur.
    /// </summary>
    public DateTime When { get; private set; }

    /// <summary>
    /// Changes a <paramref name="ExpectedWhen"/> of the expense with <paramref name="expenseKey"/>.
    /// </summary>
    /// <param name="expenseKey">A key of the expense to modify.</param>
    /// <param name="when">A date when the expense was expected to occur.</param>
    public ChangeExpenseExpectedWhen(IKey expenseKey, DateTime when)
    {
        Ensure.Condition.NotEmptyKey(expenseKey);
        Ensure.NotNull(when, "when");
        ExpenseKey = expenseKey;
        When = when;
    }
}
