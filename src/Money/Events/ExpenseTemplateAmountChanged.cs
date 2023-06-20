using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when an amount of expense template has changed.
    /// </summary>
    public class ExpenseTemplateAmountChanged : UserEvent
    {
        /// <summary>
        /// Gets an original value of the expense template.
        /// </summary>
        public Price OldValue { get; private set; }

        /// <summary>
        /// Gets a new value of the expense template.
        /// </summary>
        public Price NewValue { get; private set; }

        internal ExpenseTemplateAmountChanged(Price oldValue, Price newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
