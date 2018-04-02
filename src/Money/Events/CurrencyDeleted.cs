using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a currency is deleted (soft).
    /// </summary>
    public class CurrencyDeleted : UserEvent
    {
        /// <summary>
        /// Gets an unique code of the deleted currency.
        /// </summary>
        public string UniqueCode { get; private set; }

        internal CurrencyDeleted(string uniqueCode)
        {
            UniqueCode = uniqueCode;
        }
    }
}
