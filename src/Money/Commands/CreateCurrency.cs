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
    /// Creates a new currency with <see cref="UniqueCode"/> as a unique identifier.
    /// </summary>
    public class CreateCurrency : Command
    {
        /// <summary>
        /// Gets an unique code of the new currency.
        /// </summary>
        public string UniqueCode { get; private set; }

        /// <summary>
        /// Gets a symbol of the new currency.
        /// </summary>
        public string Symbol { get; private set; }

        /// <summary>
        /// Creates a new currency with <paramref name="uniqueCode"/> as a unique identifier.
        /// </summary>
        /// <param name="uniqueCode">An unique code of the new currency.</param>
        /// <param name="symbol">A symbol of the new currency</param>
        public CreateCurrency(string uniqueCode, string symbol)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            Ensure.NotNullOrEmpty(symbol, "symbol");
            UniqueCode = uniqueCode;
            Symbol = symbol;
        }
    }
}
