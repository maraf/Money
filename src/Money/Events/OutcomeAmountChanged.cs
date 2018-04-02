using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when an amount of outcome has changed.
    /// </summary>
    public class OutcomeAmountChanged : UserEvent
    {
        /// <summary>
        /// Gets an original value of the outcome.
        /// </summary>
        public Price OldValue { get; private set; }

        /// <summary>
        /// Gets a new value of the outcome.
        /// </summary>
        public Price NewValue { get; private set; }

        internal OutcomeAmountChanged(Price oldValue, Price newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
