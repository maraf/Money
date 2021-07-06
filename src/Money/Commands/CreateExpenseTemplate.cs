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
    /// Creates a new expense template.
    /// </summary>
    public class CreateExpenseTemplate : Command
    {
        /// <summary>
        /// Gets an amount of the expense template.
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a description of the expense template.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a category of the expense template.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Creates a new command for adding an expense template.
        /// </summary>
        /// <param name="amount">An amount of the expense template.</param>
        /// <param name="description">A description of the expense template.</param>
        /// <param name="when">A category of the expense template.</param>
        public CreateExpenseTemplate(Price amount, string description, IKey categoryKey)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Amount = amount;
            Description = description;
            CategoryKey = categoryKey;
        }
    }
}
