using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators.AutoExports
{
    /// <summary>
    /// Base attribute for defining 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class ExportLifetimeAttribute : Attribute
    {
        /// <summary>
        /// Gets <see cref="DependencyLifetime"/> for this attribute.
        /// </summary>
        /// <returns><see cref="DependencyLifetime"/> for this attribute.</returns>
        public abstract DependencyLifetime GetLifetime();
    }
}
