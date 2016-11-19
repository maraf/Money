using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging
{
    /// <summary>
    /// Enumeration of level message levels.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Minimal level for debuging purposes.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Informational level.
        /// </summary>
        Info = 2,

        /// <summary>
        /// Should be taken seriously.
        /// </summary>
        Warning = 4,

        /// <summary>
        /// Problem!
        /// </summary>
        Error = 8,

        /// <summary>
        /// Big problem!
        /// </summary>
        Fatal = 16
    }
}
