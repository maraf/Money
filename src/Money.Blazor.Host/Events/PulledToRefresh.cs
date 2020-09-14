using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a user pulled to refresh current data.
    /// </summary>
    public class PulledToRefresh : Event
    {
        /// <summary>
        /// An handler should set it to <c>true</c> to prevent default page reload.
        /// </summary>
        public bool IsHandled { get; set; }
    }
}
