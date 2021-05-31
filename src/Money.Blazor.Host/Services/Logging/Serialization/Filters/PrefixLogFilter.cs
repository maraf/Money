using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging.Serialization.Filters
{
    public class PrefixLogFilter : ILogFilter
    {
        private readonly ICollection<string> allowedScopePrefixes;
        private readonly ICollection<string> ignoredScopePrefixes;

        public PrefixLogFilter(ICollection<string> allowedScopePrefixes = null, ICollection<string> ignoredScopePrefixes = null)
        {
            this.allowedScopePrefixes = allowedScopePrefixes ?? Array.Empty<string>();
            this.ignoredScopePrefixes = ignoredScopePrefixes ?? Array.Empty<string>();
        }

        public static PrefixLogFilter Allowed(params string[] allowedScopePrefixes)
            => new PrefixLogFilter(allowedScopePrefixes: allowedScopePrefixes);

        public static PrefixLogFilter Ignored(params string[] ignoredScopePrefixes)
            => new PrefixLogFilter(ignoredScopePrefixes: ignoredScopePrefixes);

        public bool IsEnabled(string scopeName, LogLevel level)
        {
            if (allowedScopePrefixes.Count > 0)
            {
                foreach (string allowedScopePrefix in allowedScopePrefixes)
                {
                    if (scopeName.StartsWith(allowedScopePrefix))
                        return true;
                }

                return false;
            }

            foreach (string ignoredScopePrefix in ignoredScopePrefixes)
            {
                if (scopeName.StartsWith(ignoredScopePrefix))
                    return false;
            }

            return true;
        }
    }
}
