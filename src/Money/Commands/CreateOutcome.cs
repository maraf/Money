using Neptuo;
using Neptuo.Commands;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    /// <summary>
    /// Creates an outcome.
    /// </summary>
    public class CreateOutcome : Command
    {
        [CompositeVersion]
        public int Version { get; set; }

        /// <summary>
        /// Gets an amount of the outcome.
        /// </summary>
        [CompositeProperty(1)]
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a description of the outcome.
        /// </summary>
        [CompositeProperty(2)]
        public string Description { get; private set; }

        /// <summary>
        /// Gets a date and time when the outcome occured.
        /// </summary>
        [CompositeProperty(3)]
        public DateTime When { get; private set; }

        /// <summary>
        /// Gets a category key when outcome will be created.
        /// </summary>
        [CompositeProperty(4)]
        public IKey CategoryKey { get; private set; }
        
        /// <summary>
        /// Gets a <c>true</c> if this is a fixed expense.
        /// </summary>
        [CompositeProperty(5, Version = 2)]
        public bool IsFixed { get; set; }

        /// <summary>
        /// Creates an expense.
        /// </summary>
        /// <param name="amount">An amount of the outcome.</param>
        /// <param name="description">A description of the outcome.</param>
        /// <param name="when">A date and time when the outcome occured.</param>
        /// <param name="categoryKey">A category where it belongs</param>
        [CompositeConstructor]
        public CreateOutcome(Price amount, string description, DateTime when, IKey categoryKey)
        {
            Ensure.NotNull(amount, "amount");
            Ensure.NotNull(description, "description");
            Ensure.NotNull(when, "when");
            Ensure.Condition.NotEmptyKey(categoryKey);
            Amount = amount;
            Description = description;
            When = when;
            CategoryKey = categoryKey;

            Version = 1;
        }

        /// <summary>
        /// Creates an expense.
        /// </summary>
        /// <param name="amount">An amount of the outcome.</param>
        /// <param name="description">A description of the outcome.</param>
        /// <param name="when">A date and time when the outcome occured.</param>
        /// <param name="categoryKey">A category where it belongs</param>
        /// <param name="isFixed">Whether is it a fixed expense.</param>
        [CompositeConstructor(Version = 2)]
        public CreateOutcome(Price amount, string description, DateTime when, IKey categoryKey, bool isFixed)
            : this(amount, description, when, categoryKey)
        {
            IsFixed = isFixed;
            Version = 2;
        }
    }
}
