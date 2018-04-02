using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a currency exchange rate is removed.
    /// </summary>
    public class CurrencyExchangeRateRemoved : UserEvent
    {
        /// <summary>
        /// Gets an unique code of the source currency,
        /// </summary>
        public string SourceUniqueCode { get; private set; }

        /// <summary>
        /// Gets an unique code of the target currency,
        /// </summary>
        public string TargetUniqueCode { get; private set; }

        /// <summary>
        /// Gets a date from which the exchange rate is valid.
        /// </summary>
        public DateTime ValidFrom { get; private set; }

        /// <summary>
        /// Gets an exchange rate.
        /// </summary>
        public double Rate { get; private set; }

        internal CurrencyExchangeRateRemoved(string sourceUniqueCode, string targetUniqueCode, DateTime validFrom, double rate)
        {
            SourceUniqueCode = sourceUniqueCode;
            TargetUniqueCode = targetUniqueCode;
            ValidFrom = validFrom;
            Rate = rate;
        }
    }
}
