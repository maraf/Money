using Neptuo;
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
        /// <summary>
        /// Gets a amount of the expense template.
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a description of the expense template.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a category of the expense template.
        /// </summary>
        public IKey CategoryKey { get; set; }

        internal ExpenseTemplateCreated(Price amount, string description, IKey categoryKey)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Amount = amount;
            Description = description;
            CategoryKey = categoryKey;
        }
    }
}
