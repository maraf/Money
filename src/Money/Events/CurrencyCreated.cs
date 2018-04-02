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
    public class CurrencyCreated : UserEvent
    {
        /// <summary>
        /// Gets an unique code of the new currency.
        /// </summary>
        public string UniqueCode { get; private set; }

        /// <summary>
        /// Gets a symbol of the new currency.
        /// </summary>
        public string Symbol { get; private set; }

        internal CurrencyCreated(string uniqueCode, string symbol)
        {
            UniqueCode = uniqueCode;
            Symbol = symbol;
        }
    }
}
