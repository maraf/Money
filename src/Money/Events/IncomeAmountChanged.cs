using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when an amount of income has changed.
    /// </summary>
    public class IncomeAmountChanged : UserEvent
    {
        /// <summary>
        /// Gets an original value of the income.
        /// </summary>
        public Price OldValue { get; private set; }

        /// <summary>
        /// Gets a new value of the income.
        /// </summary>
        public Price NewValue { get; private set; }

        internal IncomeAmountChanged(Price oldValue, Price newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
