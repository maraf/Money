using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators.AutoExports
{
    /// <summary>
    /// Exports service as any-scope.
    /// </summary>
    public class ExportScopedAttribute : ExportLifetimeAttribute
    {
        public override DependencyLifetime GetLifetime()
        {
            return DependencyLifetime.Scope;
        }
    }
}
