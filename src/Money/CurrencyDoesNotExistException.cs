using Neptuo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// An exception raised when a requested currency doesn't exist.
    /// </summary>
    public class CurrencyDoesNotExistException : AggregateRootException
    { }
}
