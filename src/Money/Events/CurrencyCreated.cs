using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a new currency is created.
    /// </summary>
    public class CurrencyCreated : Event
    {
        /// <summary>
        /// Gets a name of the new currency.
        /// </summary>
        public string Name { get; private set; }

        internal CurrencyCreated(string name)
        {
            Name = name;
        }
    }
}
