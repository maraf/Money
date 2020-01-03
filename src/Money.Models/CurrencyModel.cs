using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model of currency.
    /// </summary>
    public class CurrencyModel : ICloneable<CurrencyModel>
    {
        /// <summary>
        /// Gets an unique currency code.
        /// </summary>
        public string UniqueCode { get; set; }

        /// <summary>
        /// Gets a currency symbol.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets a <c>true</c> if a currency is the default one.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="uniqueCode">An unique currency code.</param>
        /// <param name="symbol">A currency symbol.</param>
        /// <param name="isDefault">A <c>true</c> if a currency is the default one.</param>
        public CurrencyModel(string uniqueCode, string symbol, bool isDefault)
        {
            UniqueCode = uniqueCode;
            Symbol = symbol;
            IsDefault = isDefault;
        }

        public CurrencyModel Clone()
            => new CurrencyModel(UniqueCode, Symbol, IsDefault);
    }
}
