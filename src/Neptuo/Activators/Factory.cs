using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Factory class for the instance of <see cref="IFactory{T}"/>.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Creates new instance that uses <paramref name="getter"/> to providing instances.
        /// </summary>
        /// <typeparam name="T">The type of the object the factory will be creating.</typeparam>
        /// <param name="getter">Instances provider delegate.</param>
        /// <returns>The instance of the factory.</returns>
        public static IFactory<T> Getter<T>(Func<T> getter)
        {
            return new GetterFactory<T>(getter);
        }

        /// <summary>
        /// Creates new instance that uses <paramref name="getter"/> to providing instances.
        /// </summary>
        /// <typeparam name="T">The type of the object the factory will be creating.</typeparam>
        /// <param name="getter">Instances provider delegate.</param>
        /// <returns>The instance of the factory.</returns>
        public static IFactory<T, TContext> Getter<T, TContext>(Func<TContext, T> getter)
        {
            return new GetterFactory<T, TContext>(getter);
        }

        /// <summary>
        /// Create new instance from already created singleton object.
        /// </summary>
        /// <typeparam name="T">The type of the object the factory will be creating.</typeparam>
        /// <param name="instance">Singleton object.</param>
        /// <returns>The instance of the factory.</returns>
        public static IFactory<T> Instance<T>(T instance)
        {
            return new InstanceFactory<T>(instance);
        }

        /// <summary>
        /// Creates new instance from <paramref name="getter"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object the factory will be creating.</typeparam>
        /// <param name="getter">The delegate to access singleton object.</param>
        /// <returns>The instance of the factory.</returns>
        public static IFactory<T> Instance<T>(Func<T> getter)
        {
            return new InstanceFactory<T>(getter);
        }

        /// <summary>
        /// Creates new instance that uses default constructor for creating instances.
        /// </summary>
        /// <typeparam name="T">The type of the object the factory will be creating.</typeparam>
        /// <returns>The instance of the factory.</returns>
        public static IFactory<T> Default<T>()
            where T  : new()
        {
            return new DefaultFactory<T>();
        }
    }
}
