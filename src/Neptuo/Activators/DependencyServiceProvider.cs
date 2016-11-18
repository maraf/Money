using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Implementation of <see cref="IServiceProvider"/> using <see cref="IDependencyProvider"/>.
    /// </summary>
    public class DependencyServiceProvider : IServiceProvider
    {
        private readonly IDependencyProvider dependencyProvider;

        /// <summary>
        /// Creates new instance that resolves instances using <paramref name="dependencyProvider"/>.
        /// </summary>
        /// <param name="dependencyProvider">Instance resolution provider.</param>
        public DependencyServiceProvider(IDependencyProvider dependencyProvider)
        {
            Ensure.NotNull(dependencyProvider, "dependencyProvider");
            this.dependencyProvider = dependencyProvider;
        }

        public object GetService(Type serviceType)
        {
            return dependencyProvider.Resolve(serviceType);
        }
    }
}
