using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when symbol of a currency is changed.
    /// </summary>
    public class CurrencySymbolChanged : UserEvent
    {
        /// <summary>
        /// Gets an unique code of the currency.
        /// </summary>
        public string UniqueCode { get; private set; }

        /// <summary>
        /// Gets a new symbol of the currency.
        /// </summary>
        public string Symbol { get; private set; }

        internal CurrencySymbolChanged(string uniqueCode, string symbol)
        {
            UniqueCode = uniqueCode;
            Symbol = symbol;
        }
    }
}
