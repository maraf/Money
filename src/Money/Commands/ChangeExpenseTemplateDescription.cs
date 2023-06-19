using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    /// <summary>
    /// Changes a <see cref="Description"/> of the expense template with <see cref="ExpenseTemplateKey"/>.
    /// </summary>
    public class ChangeExpenseTemplateDescription : Command
    {
        /// <summary>
        /// Gets a key of the expense template to modify.
        /// </summary>
        public IKey ExpenseTemplateKey { get; private set; }

        /// <summary>
        /// Gets a new description of the expense template.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Changes a <paramref name="description"/> of the expense template with <paramref name="expenseTemplateKey"/>.
        /// </summary>
        /// <param name="expenseTemplateKey">A key of the expense template to modify.</param>
        /// <param name="description">A new description of the expense template.</param>
        public ChangeExpenseTemplateDescription(IKey expenseTemplateKey, string description)
        {
            Ensure.Condition.NotEmptyKey(expenseTemplateKey);
            Ensure.NotNull(description, "description");
            ExpenseTemplateKey = expenseTemplateKey;
            Description = description;
        }
    }
}
