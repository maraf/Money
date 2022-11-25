using Neptuo;
using Neptuo.Commands;
using Neptuo.Formatters.Metadata;
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
        [CompositeVersion]
        public int Version { get; set; }

        /// <summary>
        /// Gets an amount of the expense template.
        /// </summary>
        [CompositeProperty(1)]
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a description of the expense template.
        /// </summary>
        [CompositeProperty(2)]
        public string Description { get; private set; }

        /// <summary>
        /// Gets a category of the expense template.
        /// </summary>
        [CompositeProperty(3)]
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets whether the template should create fixed expenses.
        /// </summary>
        [CompositeProperty(4, Version = 2)]
        public bool IsFixed { get; private set; }

        /// <summary>
        /// Creates a new command for adding an expense template.
        /// </summary>
        /// <param name="amount">An amount of the expense template.</param>
        /// <param name="description">A description of the expense template.</param>
        /// <param name="when">A category of the expense template.</param>
        /// <param name="categoryKey">A category of the expense template.</param>
        [CompositeConstructor]
        public CreateExpenseTemplate(Price amount, string description, IKey categoryKey)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Amount = amount;
            Description = description;
            CategoryKey = categoryKey;

            Version = 1;
        }

        /// <summary>
        /// Creates a new command for adding an expense template.
        /// </summary>
        /// <param name="amount">An amount of the expense template.</param>
        /// <param name="description">A description of the expense template.</param>
        /// <param name="when">A category of the expense template.</param>
        /// <param name="categoryKey">A category of the expense template.</param>
        /// <param name="isFixed">Whether the template should create fixed expenses.</param>
        [CompositeConstructor(Version = 2)]
        public CreateExpenseTemplate(Price amount, string description, IKey categoryKey, bool isFixed)
            : this(amount, description, categoryKey)
        {
            IsFixed = isFixed;
            Version = 2;
        }
    }
}
