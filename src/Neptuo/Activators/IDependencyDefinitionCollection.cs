using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Collection of dependencies.
    /// </summary>
    public interface IDependencyDefinitionCollection : IDependencyDefinitionReadOnlyCollection
    {
        /// <summary>
        /// Registers mapping from <paramref name="requiredType"/> to <paramref name="target"/>
        /// </summary>
        /// <param name="requiredType">Required type.</param>
        /// <param name="lifetime">Lifetime of created instance.</param>
        /// <param name="target">Any supported target object.</param>
        /// <returns>Self (fluently).</returns>
        IDependencyDefinitionCollection Add(Type requiredType, DependencyLifetime lifetime, object target);
    }
}
