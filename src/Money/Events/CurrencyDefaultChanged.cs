using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a default currency was changed.
    /// </summary>
    public class CurrencyDefaultChanged : Event
    {
        /// <summary>
        /// Gets a name of the default currency.
        /// </summary>
        public string Name { get; private set; }

        internal CurrencyDefaultChanged(string name)
        {
            Name = name;
        }
    }
}
