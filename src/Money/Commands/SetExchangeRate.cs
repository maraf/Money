using Neptuo;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    /// <summary>
    /// Sets an exchange rate from <see cref="SourceUniqueCode"/> to <see cref="TargetUniqueCode"/>.
    /// It is valid from <see cref="ValidFrom"/> with exchange <see cref="Rate"/>.
    /// </summary>
    public class SetExchangeRate : Command
    {
        /// <summary>
        /// Gets a source currency unique code.
        /// </summary>
        public string SourceUniqueCode { get; private set; }

        /// <summary>
        /// Gets a target currency unique code.
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

        /// <summary>
        /// Sets an exchange rate from <paramref name="sourceUniqueCode"/> to <paramref name="targetUniqueCode"/>.
        /// It is valid from <paramref name="validFrom"/> with exchange <paramref name="rate"/>.
        /// </summary>
        /// <param name="sourceUniqueCode">A source currency unique code.</param>
        /// <param name="targetUniqueCode">A target currency unique code.</param>
        /// <param name="validFrom">A date from which the exchange rate is valid.</param>
        /// <param name="rate">An exchange rate.</param>
        public SetExchangeRate(string sourceUniqueCode, string targetUniqueCode, DateTime validFrom, double rate)
        {
            Ensure.NotNullOrEmpty(sourceUniqueCode, "sourceUniqueCode");
            Ensure.NotNullOrEmpty(targetUniqueCode, "targetUniqueCode");
            Ensure.NotNull(validFrom, "validFrom");
            Ensure.Positive(rate, "rate");
            SourceUniqueCode = sourceUniqueCode;
            TargetUniqueCode = targetUniqueCode;
            ValidFrom = validFrom;
            Rate = rate;
        }
    }
}
