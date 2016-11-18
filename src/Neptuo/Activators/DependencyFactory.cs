using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Implementation of <see cref="IActivator<T>"/> using <see cref="IDependencyProvider"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DependencyFactory<T> : IFactory<T>
    {
        private readonly IDependencyProvider dependencyProvider;

        /// <summary>
        /// Creates new instance that resolves instance using <paramref name="dependencyProvider"/>.
        /// </summary>
        /// <param name="dependencyProvider">Instance resolution provider.</param>
        public DependencyFactory(IDependencyProvider dependencyProvider)
        {
            Ensure.NotNull(dependencyProvider, "dependencyProvider");
            this.dependencyProvider = dependencyProvider;
        }

        public T Create()
        {
            return dependencyProvider.Resolve<T>();
        }
    }
}
