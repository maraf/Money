using Neptuo.Exceptions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Internals
{
    /// <summary>
    /// Extensions for the <see cref="Ensure.Exception"/>.
    /// </summary>
    internal static class _EnsureExceptionExtensions
    {
        /// <summary>
        /// Returns an instance of the <see cref="InvalidOperationException"/> with a message about undefined type of the <paramref name="handler"/>.
        /// </summary>
        public static InvalidOperationException UndefinedHandlerType(this EnsureExceptionHelper ensure, HandlerDescriptor handler)
        {
            return Ensure.Exception.InvalidOperation("The handler '{0}' is of undefined type (not plain, not envelope, not context).", handler.HandlerIdentifier);
        }
    }
}
