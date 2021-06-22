using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when an date when the income occured has changed.
    /// </summary>
    public class IncomeWhenChanged : UserEvent
    {
        /// <summary>
        /// Get a new date when the income occured.
        /// </summary>
        public DateTime When { get; private set; }

        internal IncomeWhenChanged(DateTime when)
        {
            When = when;
        }
    }
}
