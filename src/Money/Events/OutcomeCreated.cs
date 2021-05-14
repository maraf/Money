using Neptuo.Events;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when new outcome is created.
    /// </summary>
    public class OutcomeCreated : UserEvent
    {
        [CompositeVersion]
        public int CompositeVersion { get; set; }

        /// <summary>
        /// Gets a amount of the outcome.
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
        /// Gets a category where the outcome is created.
        /// </summary>
        [CompositeProperty(4)]
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets a <c>true</c> if this is a fixed expense.
        /// </summary>
        [CompositeProperty(5, Version = 2)]
        public bool IsFixed { get; set; }

        [CompositeConstructor(Version = 1)]
        internal OutcomeCreated(Price amount, string description, DateTime when, IKey categoryKey)
        {
            Amount = amount;
            Description = description;
            When = when;
            CategoryKey = categoryKey;
            CompositeVersion = 1;
        }

        [CompositeConstructor(Version = 2)]
        internal OutcomeCreated(Price amount, string description, DateTime when, IKey categoryKey, bool isFixed)
            : this(amount, description, when, categoryKey)
        {
            IsFixed = isFixed;
            CompositeVersion = 2;
        }
    }
}
