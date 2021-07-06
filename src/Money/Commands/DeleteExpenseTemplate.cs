using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    /// <summary>
    /// Deletes an expense template.
    /// </summary>
    public class DeleteExpenseTemplate : Command
    {
        /// <summary>
        /// Gets a key of the expense template to delete.
        /// </summary>
        public IKey ExpenseTemplateKey { get; private set; }

        /// <summary>
        /// Deletes an outcome with <paramref name="expenseTemplateKey"/>.
        /// </summary>
        /// <param name="expenseTemplateKey">A key of the expense template to delete.</param>
        public DeleteExpenseTemplate(IKey expenseTemplateKey)
        {
            Ensure.Condition.NotEmptyKey(expenseTemplateKey);
            ExpenseTemplateKey = expenseTemplateKey;
        }
    }
}
