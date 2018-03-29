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
    /// Changes a <see cref="NewSymbol"/> of the currency with <see cref="UniqueCode"/>.
    /// </summary>
    public class ChangeCurrencySymbol : Command
    {
        /// <summary>
        /// Gets an unique code of the currency to change symbol for.
        /// </summary>
        public string UniqueCode { get; private set; }

        /// <summary>
        /// Gets a new symbol for the currency.
        /// </summary>
        public string NewSymbol { get; private set; }

        /// <summary>
        /// Changes a <paramref name="newSymbol"/> of the currency with <paramref name="uniqueCode"/>.
        /// </summary>
        /// <param name="uniqueCode">An unique code of the currency to change symbol for.</param>
        /// <param name="newSymbol">A new symbol for the currency.</param>
        public ChangeCurrencySymbol(string uniqueCode, string newSymbol)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            Ensure.NotNullOrEmpty(newSymbol, "newSymbol");
            UniqueCode = uniqueCode;
            NewSymbol = newSymbol;
        }
    }
}
