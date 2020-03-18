using Money.Events;
using Neptuo;
using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class ApiVersionChecker
    {
        private readonly Version minVersion = new Version(1, 3, 0, 0);
        private readonly Version maxVersion = new Version(2, 0, 0, 0);

        private readonly IEventDispatcher events;
        private Version last;

        public ApiVersionChecker(IEventDispatcher events)
        {
            Neptuo.Ensure.NotNull(events, "events");
            this.events = events;
        }

        public bool IsPassed(Version version)
        {
            if (version != last)
                events.PublishAsync(new ApiVersionChanged(last = version));

            return version >= minVersion && version < maxVersion;
        }

        public void Ensure(Version version)
        {
            if (!IsPassed(version))
                throw new NotSupportedApiVersionException(version);
        }
    }
}
