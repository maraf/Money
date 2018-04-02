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
    public class CurrencyDefaultChanged : UserEvent
    {
        /// <summary>
        /// Gets an unique code of the default currency.
        /// </summary>
        public string UniqueCode { get; private set; }

        internal CurrencyDefaultChanged(string uniqueCode)
        {
            UniqueCode = uniqueCode;
        }
    }
}
