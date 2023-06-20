using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a fixed state of the expense template has changed.
    /// </summary>
    public class ExpenseTemplateFixedChanged : UserEvent
    {
        /// <summary>
        /// Gets a new value of the fixed state.
        /// </summary>
        public bool IsFixed { get; private set; }

        internal ExpenseTemplateFixedChanged(bool isFixed)
        {
            IsFixed = isFixed;
        }
    }
}
