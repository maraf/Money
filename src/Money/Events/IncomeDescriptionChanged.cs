using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a description of the income has changed.
    /// </summary>
    public class IncomeDescriptionChanged : UserEvent
    {
        /// <summary>
        /// Gets a new value of the description.
        /// </summary>
        public string Description { get; private set; }

        internal IncomeDescriptionChanged(string description)
        {
            Description = description;
        }
    }
}
