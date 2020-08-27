using Neptuo;
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
    /// An event raised when server API version changed.
    /// </summary>
    public class ApiVersionChanged : Event
    {
        public Version ApiVersion { get; }

        public ApiVersionChanged(Version apiVerion)
        {
            Ensure.NotNull(apiVerion, "apiVerion");
            ApiVersion = apiVerion;
        }
    }
}
