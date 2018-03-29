using Money.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// A component for converting price between currencies.
    /// </summary>
    public interface IPriceConverter
    {
        /// <summary>
        /// Creates a zero price in default currency.
        /// </summary>
        /// <returns></returns>
        Price ZeroDefault();

        /// <summary>
        /// Converts <paramref name="price"/> to default currency.
        /// </summary>
        /// <param name="price">A price to convert.</param>
        /// <returns>A converted price.</returns>
        Price ToDefault(IPriceFixed price);
    }
}
