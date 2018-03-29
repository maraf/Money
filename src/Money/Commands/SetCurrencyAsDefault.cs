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
    /// Sets a <see cref="UniqueCode"/> as a default currency.
    /// </summary>
    public class SetCurrencyAsDefault : Command
    {
        /// <summary>
        /// Gets an unique code of the new currency.
        /// </summary>
        public string UniqueCode { get; private set; }

        /// <summary>
        /// Sets a <paramref name="uniqueCode"/> as a default currency.
        /// </summary>
        /// <param name="uniqueCode">An unique code of the currency.</param>
        public SetCurrencyAsDefault(string uniqueCode)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            UniqueCode = uniqueCode;
        }
    }
}
