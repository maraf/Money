using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Neptuo.Activators
{
    /// <summary>
    /// Common extensions for <see cref="IDependencyProvider"/>.
    /// </summary>
    public static class _DependencyProviderExtensions
    {
        /// <summary>
        /// Resolves instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Required type.</typeparam>
        /// <returns>Instance of <typeparamref name="T"/>; if it's not possible to create instance.</returns>
        public static T Resolve<T>(this IDependencyProvider dependencyProvider)
        {
            Ensure.NotNull(dependencyProvider, "dependencyProvider");
            return (T)dependencyProvider.Resolve(typeof(T));
        }

        /// <summary>
        /// Resolves instance of <paramref name="requiredType"/> where <paramref name="requiredType"/> must be assignable to <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">Returned type.</typeparam>
        /// <param name="requiredType">Required type.</param>
        /// <returns>Instance of <paramref name="requiredType"/>; if it's not possible to create instance.</returns>
        public static T Resolve<T>(this IDependencyProvider dependencyProvider, Type requiredType)
        {
            Ensure.NotNull(dependencyProvider, "dependencyProvider");
            return (T)dependencyProvider.Resolve(requiredType);
        }

        /// <summary>
        /// Tries to resolve <typeparamref name="T"/> from <paramref name="dependencyProvider"/>.
        /// </summary>
        /// <typeparam name="T">Type of service to resolved.</typeparam>
        /// <param name="dependencyProvider">Resolution provider.</param>
        /// <param name="instance">Obtained instance.</param>
        /// <returns><c>true</c> when resolution was successful; <c>false</c> otherwise.</returns>
        public static bool TryResolve<T>(this IDependencyProvider dependencyProvider, out T instance)
        {
            Ensure.NotNull(dependencyProvider, "dependencyProvider");
            Type requiredType = typeof(T);
            IDependencyDefinition definition;
            if (dependencyProvider.Definitions.TryGet(requiredType, out definition))
            {
                if (definition.IsResolvable)
                {
                    try
                    {
                        instance = (T)dependencyProvider.Resolve(requiredType);
                        return true;
                    }
                    catch (DependencyResolutionFailedException)
                    {
                        // Catch DependencyResolutionException and let it return false.
                    }
                }
            } 
            else if (!requiredType.GetTypeInfo().IsAbstract && !requiredType.GetTypeInfo().IsInterface)
            {
                try
                {
                    instance = (T)dependencyProvider.Resolve(requiredType);
                    return true;
                }
                catch (DependencyResolutionFailedException)
                {
                    // Catch DependencyResolutionException and let it return false.
                }
            }

            instance = default(T);
            return false;
        }
    }
}
