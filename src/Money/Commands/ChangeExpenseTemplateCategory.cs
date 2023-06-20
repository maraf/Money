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
    /// Changes a category of the expense template with <see cref="ExpenseTemplateKey"/>.
    /// </summary>
    public class ChangeExpenseTemplateCategory : Command, IExpenseTemplateCommand
    {
        /// <summary>
        /// Gets a key of the expense template to modify.
        /// </summary>
        public IKey ExpenseTemplateKey { get; private set; }

        /// <summary>
        /// Gets a category key.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Changes a <paramref name="when"/> of the expense template with <paramref name="expenseTemplateKey"/>.
        /// </summary>
        /// <param name="expenseTemplateKey">A key of the expense template to modify.</param>
        /// <param name="categoryKey">A category key.</param>
        public ChangeExpenseTemplateCategory(IKey expenseTemplateKey, IKey categoryKey)
        {
            Ensure.Condition.NotEmptyKey(expenseTemplateKey);
            Ensure.NotNull(categoryKey, "categoryKey");
            ExpenseTemplateKey = expenseTemplateKey;
            CategoryKey = categoryKey;
        }
    }
}
