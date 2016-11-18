using Neptuo.Exceptions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Activators extensions for <see cref="EnsureExceptionHelper"/>.
    /// </summary>
    public static class _EnsureExceptionExtensions
    {
        /// <summary>
        /// Creates new instance of <see cref="DependencyResolutionFailedException"/> problem with resolving <paramref name="requiredType"/> occurred.
        /// </summary>
        /// <typeparam name="T">Type of service resolution failed.</typeparam>
        /// <param name="exception">Exception helper.</param>
        /// <param name="requiredType">Service type where resolution failed.</param>
        /// <returns>New instance of <see cref="DependencyResolutionFailedException"/> problem with resolving <paramref name="requiredType"/> occurred.</returns>
        public static DependencyResolutionFailedException NotResolvable(this EnsureExceptionHelper exception, Type requiredType)
        {
            Ensure.NotNull(exception, "exception");
            Ensure.NotNull(requiredType, "requiredType");
            return new DependencyResolutionFailedException(String.Format("Problem resolving type '{0}'.", requiredType.FullName));
        }

        /// <summary>
        /// Creates new instance of <see cref="DependencyResolutionFailedException"/> problem with resolving <paramref name="requiredType"/> occurred.
        /// </summary>
        /// <typeparam name="T">Type of service resolution failed.</typeparam>
        /// <param name="exception">Exception helper.</param>
        /// <param name="requiredType">Service type where resolution failed.</param>
        /// <param name="inner">Source exception.</param>
        /// <returns>New instance of <see cref="DependencyResolutionFailedException"/> problem with resolving <paramref name="requiredType"/> occurred.</returns>
        public static DependencyResolutionFailedException NotResolvable(this EnsureExceptionHelper exception, Type requiredType, Exception inner)
        {
            Ensure.NotNull(exception, "exception");
            Ensure.NotNull(requiredType, "requiredType");
            return new DependencyResolutionFailedException(String.Format("Problem resolving type '{0}'.", requiredType.FullName), inner);
        }
    }
}
