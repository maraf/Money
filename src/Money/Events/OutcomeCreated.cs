using Neptuo.Events;
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
    public class OutcomeCreated : Event
    {
        /// <summary>
        /// Gets a amount of the outcome.
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a description of the outcome.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a date and time when the outcome occured.
        /// </summary>
        public DateTime When { get; private set; }

        /// <summary>
        /// Gets a category where the outcome is created.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        internal OutcomeCreated(Price amount, string description, DateTime when, IKey categoryKey)
        {
            Amount = amount;
            Description = description;
            When = when;
            CategoryKey = categoryKey;
        }
    }
}
