using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model of single expense template.
    /// </summary>
    public class ExpenseTemplateModel
    {
        /// <summary>
        /// Gets a key of the expense template.
        /// </summary>
        public IKey Key { get; private set; }

        /// <summary>
        /// Gets an amount of the expense template.
        /// </summary>
        public Price Amount { get; set; }

        /// <summary>
        /// Gets a description of the expense template.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets a date when the expense template ocured.
        /// </summary>
        public IKey CategoryKey { get; set; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="key">A key of the expense template.</param>
        /// <param name="amount">An amount of the expense template.</param>
        /// <param name="description">A description of the expense template.</param> 
        /// <param name="categoryKey">A category of the expense template.</param>
        public ExpenseTemplateModel(IKey key, Price amount, string description, IKey categoryKey)
        {
            Ensure.Condition.NotEmptyKey(key);
            Ensure.NotNull(categoryKey, "categoryKey");
            Key = key;
            Amount = amount;
            CategoryKey = categoryKey;
            Description = description;
        }
    }
}
