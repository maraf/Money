using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A price with fixed datetime.
    /// </summary>
    public interface IPriceFixed
    {
        /// <summary>
        /// Gets an amount.
        /// </summary>
        Price Amount { get; }

        /// <summary>
        /// Gets a date & time when occured.
        /// </summary>
        DateTime When { get; }
    }
}
