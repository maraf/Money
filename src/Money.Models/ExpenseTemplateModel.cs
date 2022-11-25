using Neptuo;
using Neptuo.Formatters.Metadata;
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
    public class ExpenseTemplateModel : ICloneable<ExpenseTemplateModel>
    {
        [CompositeVersion]
        public int Version { get; private set; }

        /// <summary>
        /// Gets a key of the expense template.
        /// </summary>
        [CompositeProperty(1)]
        [CompositeProperty(1, Version = 2)]
        public IKey Key { get; private set; }

        /// <summary>
        /// Gets an amount of the expense template.
        /// </summary>
        [CompositeProperty(2)]
        [CompositeProperty(2, Version = 2)]
        public Price Amount { get; set; }

        /// <summary>
        /// Gets a description of the expense template.
        /// </summary>
        [CompositeProperty(3)]
        [CompositeProperty(3, Version = 2)]
        public string Description { get; set; }

        /// <summary>
        /// Gets a date when the expense template ocured.
        /// </summary>
        [CompositeProperty(4)]
        [CompositeProperty(4, Version = 2)]
        public IKey CategoryKey { get; set; }

        /// <summary>
        /// Gets whether the template should create fixed expenses.
        /// </summary>
        [CompositeProperty(5, Version = 2)]
        public bool IsFixed { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="key">A key of the expense template.</param>
        /// <param name="amount">An amount of the expense template.</param>
        /// <param name="description">A description of the expense template.</param> 
        /// <param name="categoryKey">A category of the expense template.</param>
        [CompositeConstructor]
        public ExpenseTemplateModel(IKey key, Price amount, string description, IKey categoryKey)
        {
            Ensure.Condition.NotEmptyKey(key);
            Ensure.NotNull(categoryKey, "categoryKey");
            Key = key;
            Amount = amount;
            CategoryKey = categoryKey;
            Description = description;
            Version = 1;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="key">A key of the expense template.</param>
        /// <param name="amount">An amount of the expense template.</param>
        /// <param name="description">A description of the expense template.</param> 
        /// <param name="categoryKey">A category of the expense template.</param>
        /// <param name="isFixed">Whether the template should create fixed expenses.</param>
        [CompositeConstructor(Version = 2)]
        public ExpenseTemplateModel(IKey key, Price amount, string description, IKey categoryKey, bool isFixed)
            : this(key, amount, description, categoryKey)
        {
            IsFixed = isFixed;
            Version = 2;
        }

        public ExpenseTemplateModel Clone() => Version == 1
            ? new ExpenseTemplateModel(Key, Amount, Description, CategoryKey)
            : new ExpenseTemplateModel(Key, Amount, Description, CategoryKey, IsFixed);
    }
}
