using Neptuo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// An exception raised when a currency to delete is set as default.
    /// </summary>
    public class CantDeleteDefaultCurrencyException : AggregateRootException
    { }
}
