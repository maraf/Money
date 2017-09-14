using Money.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    /// <summary>
    /// A component for converting price between currencies.
    /// </summary>
    public interface IPriceConverter
    {
        /// <summary>
        /// Converts <paramref name="price"/> to default currency.
        /// </summary>
        /// <param name="price">A price to convert.</param>
        /// <returns>A converted price.</returns>
        Price ToDefault(IPriceFixed price);
    }
}
