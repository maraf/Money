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
    public class CurrencyExchangeRateSet : UserEvent
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

        internal CurrencyExchangeRateSet(string sourceUniqueCode, string targetUniqueCode, DateTime validFrom, double rate)
        {
            SourceUniqueCode = sourceUniqueCode;
            TargetUniqueCode = targetUniqueCode;
            ValidFrom = validFrom;
            Rate = rate;
        }

        public override bool Equals(object obj)
        {
            CurrencyExchangeRateSet other = obj as CurrencyExchangeRateSet;
            if (other == null)
                return false;

            if (SourceUniqueCode != other.SourceUniqueCode)
                return false;

            if (TargetUniqueCode != other.TargetUniqueCode)
                return false;

            if (ValidFrom != other.ValidFrom)
                return false;

            if (Rate != other.Rate)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 37;
            hash += 7 * SourceUniqueCode.GetHashCode();
            hash += 7 * TargetUniqueCode.GetHashCode();
            hash += 7 * ValidFrom.GetHashCode();
            hash += 7 * Rate.GetHashCode();
            hash += 7 * UserKey.GetHashCode();
            return hash;
        }
    }
}
