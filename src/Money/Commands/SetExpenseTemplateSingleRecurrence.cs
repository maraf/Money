using Neptuo.Models.Keys;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Commands;

namespace Money.Commands;

public class SetExpenseTemplateSingleRecurrence : Command, IExpenseTemplateCommand
{
    /// <summary>
    /// Gets a key of the expense template to modify.
    /// </summary>
    public IKey ExpenseTemplateKey { get; private set; }

    /// <summary>
    /// Gets a due date of the expense template.
    /// </summary>
    public DateTime DueDate { get; private set; }

    public SetExpenseTemplateSingleRecurrence(IKey expenseTemplateKey, DateTime dueDate)
    {
        Ensure.Condition.NotEmptyKey(expenseTemplateKey);
        ExpenseTemplateKey = expenseTemplateKey;
        DueDate = dueDate;
    }
}
