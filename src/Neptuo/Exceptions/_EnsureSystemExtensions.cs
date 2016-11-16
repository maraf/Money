using Neptuo.Exceptions.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// Extensions for system exceptions.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class _EnsureSystemExtensions
    {
        /// <summary>
        /// Creates exception <see cref="NotImplementedException"/> for argument <paramref name="argumentName"/> 
        /// and optional message formatted from <paramref name="format"/> and <paramref name="formatParameters"/>.
        /// </summary>
        /// <param name="guard"></param>
        /// <param name="argumentName"></param>
        /// <param name="format"></param>
        /// <param name="formatParameters"></param>
        /// <returns><see cref="NotImplementedException"/>.</returns>
        [DebuggerStepThrough]
        public static NotImplementedException NotImplemented(this EnsureExceptionHelper guard, string format = null, params object[] formatParameters)
        {
            Ensure.NotNull(guard, "guard");

            if (String.IsNullOrEmpty(format))
                return new NotImplementedException();

            return new NotImplementedException(String.Format(format, formatParameters));
        }

        /// <summary>
        /// Creates exception <see cref="NotSupportedException"/> for argument <paramref name="argumentName"/> 
        /// and optional message formatted from <paramref name="format"/> and <paramref name="formatParameters"/>.
        /// </summary>
        /// <param name="guard"></param>
        /// <param name="argumentName"></param>
        /// <param name="format"></param>
        /// <param name="formatParameters"></param>
        /// <returns><see cref="NotSupportedException"/>.</returns>
        [DebuggerStepThrough]
        public static NotSupportedException NotSupported(this EnsureExceptionHelper guard, string format = null, params object[] formatParameters)
        {
            Ensure.NotNull(guard, "guard");

            if (String.IsNullOrEmpty(format))
                return new NotSupportedException();

            return new NotSupportedException(String.Format(format, formatParameters));
        }

        /// <summary>
        /// Creates exception <see cref="InvalidOperationException"/> for argument <paramref name="argumentName"/> 
        /// and message formatted from <paramref name="format"/> and <paramref name="formatParameters"/>.
        /// </summary>
        /// <param name="guard"></param>
        /// <param name="argumentName"></param>
        /// <param name="format"></param>
        /// <param name="formatParameters"></param>
        /// <returns><see cref="InvalidOperationException"/>.</returns>
        public static InvalidOperationException InvalidOperation(this EnsureExceptionHelper guard, string format, params object[] formatParameters)
        {
            Ensure.NotNull(guard, "guard");
            Ensure.NotNullOrEmpty(format, "format");
            return new InvalidOperationException(String.Format(format, formatParameters));
        }
    }
}
