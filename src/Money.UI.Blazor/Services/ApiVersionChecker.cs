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
        public bool IsPassed(Version version)
        {
            // TODO: Check for compatibility.
            return false;
        }

        public void Ensure(Version version)
        {
            if (!IsPassed(version))
                throw new NotSupportedApiVersionException(version);
        }
    }
}
