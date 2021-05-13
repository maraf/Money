using Neptuo;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model of a single outcome for overview.
    /// </summary>
    public class OutcomeOverviewModel
    {
        [CompositeVersion]
        public int Version { get; private set; }

        /// <summary>
        /// Gets a key of the outcome.
        /// </summary>
        [CompositeProperty(1)]
        [CompositeProperty(1, Version = 2)]
        public IKey Key { get; private set; }

        /// <summary>
        /// Gets an amount of the outcome.
        /// </summary>
        [CompositeProperty(2)]
        [CompositeProperty(2, Version = 2)]
        public Price Amount { get; set; }

        /// <summary>
        /// Gets a date when the outcome ocured.
        /// </summary>
        [CompositeProperty(3)]
        [CompositeProperty(3, Version = 2)]
        public DateTime When { get; set; }

        /// <summary>
        /// Gets a description of the outcome.
        /// </summary>
        [CompositeProperty(4)]
        [CompositeProperty(4, Version = 2)]
        public string Description { get; set; }

        /// <summary>
        /// Gets a key of a category.
        /// </summary>
        [CompositeProperty(5)]
        [CompositeProperty(5, Version = 2)]
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets a <c>true</c> if this is a fixed expense.
        /// </summary>
        [CompositeProperty(6, Version = 2)]
        public bool IsFixed { get; set; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="key">A key of the outcome.</param>
        /// <param name="amount">An amount of the outcome.</param>
        /// <param name="when">A date when the outcome ocured.</param>
        /// <param name="description">A description of the outcome.</param> 
        /// <param name="categoryKey">A key of a category.</param>
        [CompositeConstructor]
        public OutcomeOverviewModel(IKey key, Price amount, DateTime when, string description, IKey categoryKey)
        {
            Ensure.Condition.NotEmptyKey(key);
            Ensure.NotNull(amount, "amount");
            Ensure.Condition.NotEmptyKey(categoryKey);
            Key = key;
            Amount = amount;
            When = when;
            Description = description;
            CategoryKey = categoryKey;

            Version = 1;
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="key">A key of the outcome.</param>
        /// <param name="amount">An amount of the outcome.</param>
        /// <param name="when">A date when the outcome ocured.</param>
        /// <param name="description">A description of the outcome.</param> 
        /// <param name="categoryKey">A key of a category.</param>
        /// <param name="isFixed">Whether is it a fixed expense.</param>
        [CompositeConstructor(Version = 2)]
        public OutcomeOverviewModel(IKey key, Price amount, DateTime when, string description, IKey categoryKey, bool isFixed)
            : this(key, amount, when, description, categoryKey)
        {
            IsFixed = isFixed;

            Version = 2;
        }
    }
}
