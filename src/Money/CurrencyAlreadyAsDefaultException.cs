using Neptuo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// An exception raised when a user tries to set a currency as default, but that currency already is a default one.
    /// </summary>
    public class CurrencyAlreadyAsDefaultException : AggregateRootException
    { }
}
