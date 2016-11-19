using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging.Serialization.Filters
{
    /// <summary>
    /// Implementation of <see cref="ILogFilter"/> that allows everything.
    /// </summary>
    public class AllowedLogFilter : ILogFilter
    {
        public bool IsEnabled(string scopeName, LogLevel level)
        {
            return true;
        }
    }
}
