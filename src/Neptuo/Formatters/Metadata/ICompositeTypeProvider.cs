using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// The provider of composite type descriptors.
    /// </summary>
    public interface ICompositeTypeProvider
    {
        /// <summary>
        /// Tries to get composite type descritor for <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to get descriptor for.</param>
        /// <param name="definition">The composite type descriptor.</param>
        /// <returns><c>true</c> if <paramref name="definition"/> is provided; <c>false</c> otherwise.</returns>
        bool TryGet(Type type, out CompositeType definition);

        /// <summary>
        /// Tries to get composite type descritor for <paramref name="typeName"/>.
        /// </summary>
        /// <param name="typeName">The name of the type to get descriptor for.</param>
        /// <param name="definition">The composite type descriptor.</param>
        /// <returns><c>true</c> if <paramref name="definition"/> is provided; <c>false</c> otherwise.</returns>
        bool TryGet(string typeName, out CompositeType definition);
    }
}
