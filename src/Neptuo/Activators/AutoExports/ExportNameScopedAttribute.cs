using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators.AutoExports
{
    /// <summary>
    /// Exports service in named-scope.
    /// </summary>
    public class ExportNameScopedAttribute : ExportLifetimeAttribute
    {
        /// <summary>
        /// Scope name.
        /// </summary>
        public string ScopeName { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="scopeName">Scope name.</param>
        public ExportNameScopedAttribute(string scopeName)
        {
            ScopeName = scopeName;
        }

        public override DependencyLifetime GetLifetime()
        {
            return DependencyLifetime.NameScope(ScopeName);
        }
    }
}
