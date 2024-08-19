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
/// Clears the recurrence of the expense template with <see cref="ExpenseTemplateKey"/>.
/// </summary>
public class ClearExpenseTemplateRecurrence : Command, IExpenseTemplateCommand, IAggregateRootCommand
{
    /// <summary>
    /// Gets a key of the expense template to modify.
    /// </summary>
    public IKey ExpenseTemplateKey { get; private set; }

    IKey IAggregateRootCommand.AggregateKey => ExpenseTemplateKey;

    public ClearExpenseTemplateRecurrence(IKey expenseTemplateKey)
    {
        Ensure.Condition.NotEmptyKey(expenseTemplateKey);
        ExpenseTemplateKey = expenseTemplateKey;
    }
}
