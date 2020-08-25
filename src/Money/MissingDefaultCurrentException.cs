using Neptuo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// An exception raised when a user's default currency is not set.
    /// </summary>
    public class MissingDefaultCurrentException : AggregateRootException
    { }
}
