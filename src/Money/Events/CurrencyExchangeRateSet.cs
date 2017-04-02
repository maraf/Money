using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a currency exchange rate is set.
    /// </summary>
    public class CurrencyExchangeRateSet : Event
    {
        /// <summary>
        /// Gets a name of the source currency,
        /// </summary>
        public string SourceName { get; private set; }

        /// <summary>
        /// Gets a name of the target currency,
        /// </summary>
        public string TargetName { get; private set; }

        /// <summary>
        /// Gets a date from which the exchange rate is valid.
        /// </summary>
        public DateTime ValidFrom { get; private set; }

        /// <summary>
        /// Gets an exchange rate.
        /// </summary>
        public decimal Rate { get; private set; }

        internal CurrencyExchangeRateSet(string sourceName, string targetName, DateTime validFrom, decimal rate)
        {
            SourceName = sourceName;
            TargetName = targetName;
            ValidFrom = validFrom;
            Rate = rate;
        }
    }
}
