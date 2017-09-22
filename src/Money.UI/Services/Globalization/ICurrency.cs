using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services.Globalization
{
    /// <summary>
    /// A concrete currency with support for formatting a value.
    /// </summary>
    public interface ICurrency
    {
        /// <summary>
        /// Gets a currency unique code.
        /// </summary>
        string UniqueCode { get; }

        /// <summary>
        /// Gets a currency symbol.
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// Formats <paramref name="amount"/> as money string.
        /// </summary>
        /// <param name="amount">An amount.</param>
        /// <returns>Formatted <paramref name="amount"/>.</returns>
        string Format(decimal amount);

        /// <summary>
        /// Creates a new <see cref="ICurre"/> with <paramref name="symbol"/> for cyrrency symbol.
        /// </summary>
        /// <param name="symbol">A symbol of currency.</param>
        /// <returns>A new <see cref="ICurre"/> with <paramref name="symbol"/> for cyrrency symbol.</returns>
        ICurrency ForCustomSymbol(string symbol);
    }
}
