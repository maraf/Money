using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a description of the outcome has changed.
    /// </summary>
    public class OutcomeDescriptionChanged : UserEvent
    {
        /// <summary>
        /// Gets a new value of the description.
        /// </summary>
        public string Description { get; private set; }

        internal OutcomeDescriptionChanged(string description)
        {
            Description = description;
        }
    }
}
