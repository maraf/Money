using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when an date when the outcome occured has changed.
    /// </summary>
    public class OutcomeWhenChanged : UserEvent
    {
        /// <summary>
        /// Get a new date when the outcome occured.
        /// </summary>
        public DateTime When { get; private set; }

        internal OutcomeWhenChanged(DateTime when)
        {
            When = when;
        }
    }
}
