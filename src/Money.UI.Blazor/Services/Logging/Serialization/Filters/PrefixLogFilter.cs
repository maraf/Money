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
        private readonly ICollection<string> ignoredScopePrefixes;

        public PrefixLogFilter(params string[] ignoredScopePrefixes)
        {
            Ensure.NotNull(ignoredScopePrefixes, "ignoredScopePrefixes");
            this.ignoredScopePrefixes = ignoredScopePrefixes;
        }

        public bool IsEnabled(string scopeName, LogLevel level)
        {
            foreach (string ignoredScopePrefix in ignoredScopePrefixes)
            {
                if (scopeName.StartsWith(ignoredScopePrefix))
                    return false;
            }

            return true;
        }
    }
}
