using Neptuo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// An exception raised when a user tries to create a currency with name that already exists.
    /// </summary>
    public class CurrencyAlreadyExistsException : AggregateRootException
    { }
}
