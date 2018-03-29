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
    /// Deletes (soft) currency with <see cref="UniqueCode"/> as a unique identifier.
    /// </summary>
    public class DeleteCurrency : Command
    {
        /// <summary>
        /// Gets an unique code of the new currency.
        /// </summary>
        public string UniqueCode { get; private set; }

        /// <summary>
        /// Deletes (soft) currency with <paramref name="uniqueCode"/>.
        /// </summary>
        /// <param name="uniqueCode">An unique code for the currency to delete.</param>
        public DeleteCurrency(string uniqueCode)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            UniqueCode = uniqueCode;
        }
    }
}
