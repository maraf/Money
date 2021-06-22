using Neptuo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// An exception raised when trying to modify an income that is already deleted.
    /// </summary>
    public class IncomeAlreadyDeletedException : AggregateRootException
    { }
}
