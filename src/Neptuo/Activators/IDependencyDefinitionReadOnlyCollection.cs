using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Read-only collection of dependency definitinons.
    /// </summary>
    public interface IDependencyDefinitionReadOnlyCollection
    {
        /// <summary>
        /// Tries to find dependency definition for resolving type <paramref name="requiredType"/>.
        /// </summary>
        /// <param name="requiredType">Resolving type.</param>
        /// <param name="definition">Depedency definition.</param>
        /// <returns><c>true</c> if collection contains definition for <paramref name="requiredType"/>; <c>false</c> otherwise.</returns>
        bool TryGet(Type requiredType, out IDependencyDefinition definition);
    }
}
