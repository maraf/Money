using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neptuo.Activators
{
    /// <summary>
    /// Service locator with hierarchy support.
    /// </summary>
    public interface IDependencyProvider : IDisposable
    {
        /// <summary>
        /// Returns name of the current scope.
        /// </summary>
        string ScopeName { get; }

        /// <summary>
        /// Collection of current dependency definitions.
        /// </summary>
        IDependencyDefinitionReadOnlyCollection Definitions { get; }

        /// <summary>
        /// Creates new child container based on this provider.
        /// </summary>
        /// <param name="name">Name for the scope.</param>
        /// <returns>New child container based on this provider.</returns>
        IDependencyContainer Scope(string name);

        /// <summary>
        /// Resolves instance of <paramref name="requiredType"/>.
        /// </summary>
        /// <param name="requiredType">Required type.</param>
        /// <returns>Instance of <paramref name="requiredType"/>; if it's not possible to create instance.</returns>
        object Resolve(Type requiredType);
    }
}
