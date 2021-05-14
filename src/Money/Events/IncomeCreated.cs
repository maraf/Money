using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when new a income is created.
    /// </summary>
    public class IncomeCreated : UserEvent
    {
        /// <summary>
        /// Gets a amount of the income.
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a description of the income.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a date and time when the income occured.
        /// </summary>
        public DateTime When { get; private set; }

        internal IncomeCreated(Price amount, string description, DateTime when)
        {
            Amount = amount;
            Description = description;
            When = when;
        }
    }
}
