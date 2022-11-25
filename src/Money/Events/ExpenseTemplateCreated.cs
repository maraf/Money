using Neptuo;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when new a expense template is created.
    /// </summary>
    public class ExpenseTemplateCreated : UserEvent
    {
        [CompositeVersion]
        public int CompositeVersion { get; set; }

        /// <summary>
        /// Gets a amount of the expense template.
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
        public IKey CategoryKey { get; set; }

        /// <summary>
        /// Gets whether the template should create fixed expenses.
        /// </summary>
        [CompositeProperty(4, Version = 2)]
        public bool IsFixed { get; private set; }

        [CompositeConstructor(Version = 1)]
        internal ExpenseTemplateCreated(Price amount, string description, IKey categoryKey)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Amount = amount;
            Description = description;
            CategoryKey = categoryKey;
            CompositeVersion = 1;
        }

        [CompositeConstructor(Version = 2)]
        internal ExpenseTemplateCreated(Price amount, string description, IKey categoryKey, bool isFixed)
            : this(amount, description, categoryKey)
        {
            IsFixed = isFixed;
            CompositeVersion = 2;
        }
    }
}
