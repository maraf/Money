using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Describes single dependency container registration.
    /// </summary>
    public interface IDependencyDefinition
    {
        /// <summary>
        /// Type to be resolved by.
        /// </summary>
        Type RequiredType { get; }

        /// <summary>
        /// Lifetime of instances.
        /// </summary>
        DependencyLifetime Lifetime { get; }

        /// <summary>
        /// Registration target.
        /// Can be of any supported type.
        /// </summary>
        object Target { get; }

        /// <summary>
        /// Whether can be resolved in current scope.
        /// </summary>
        /// <remarks>
        /// Not necessarily means that resolution will succeed, only means that there is registration and
        /// this registration is valid inside current scope chain.
        /// </remarks>
        bool IsResolvable { get; }
    }
}
