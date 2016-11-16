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
    /// Extensions for argument exceptions.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class _EnsureArgumentExtensions
    {
        /// <summary>
        /// Creates exception <see cref="ArgumentException"/> for argument <paramref name="argumentName"/> 
        /// and message formatted from <paramref name="format"/> and <paramref name="formatParameters"/>.
        /// </summary>
        /// <param name="guard"></param>
        /// <param name="argumentName"></param>
        /// <param name="format"></param>
        /// <param name="formatParameters"></param>
        /// <returns><see cref="ArgumentException"/>.</returns>
        [DebuggerStepThrough]
        public static ArgumentException Argument(this EnsureExceptionHelper guard, string argumentName, string format, params object[] formatParameters)
        {
            Ensure.NotNull(guard, "guard");
            Ensure.NotNullOrEmpty(argumentName, "argumentName");
            Ensure.NotNullOrEmpty(format, "format");
            return new ArgumentException(argumentName, String.Format(format, formatParameters));
        }
        
        /// <summary>
        /// Creates exception <see cref="ArgumentNullException"/> with argument <paramref name="argumentName"/> 
        /// and message formatted from <paramref name="format"/> and <paramref name="formatParameters"/>.
        /// </summary>
        /// <param name="guard">Exception helper.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <param name="format">Message or format string.</param>
        /// <param name="formatParameters">Optional format string parameters.</param>
        /// <returns><see cref="ArgumentNullException"/>.</returns>
        [DebuggerStepThrough]
        public static ArgumentNullException ArgumentNull(this EnsureExceptionHelper guard, string argumentName, string format, params object[] formatParameters)
        {
            Ensure.NotNull(guard, "guard");
            Ensure.NotNullOrEmpty(argumentName, "argumentName");
            Ensure.NotNullOrEmpty(format, "format");
            return new ArgumentNullException(argumentName, String.Format(format, formatParameters));
        }

        /// <summary>
        /// Creates exception <see cref="ArgumentOutOfRangeException"/> with argument <paramref name="argumentName"/> 
        /// and message formatted from <paramref name="format"/> and <paramref name="formatParameters"/>.
        /// </summary>
        /// <param name="guard">Exception helper.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <param name="format">Message or format string.</param>
        /// <param name="formatParameters">Optional format string parameters.</param>
        /// <returns><see cref="ArgumentOutOfRangeException"/>.</returns>
        [DebuggerStepThrough]
        public static ArgumentOutOfRangeException ArgumentOutOfRange(this EnsureExceptionHelper guard, string argumentName, string format, params object[] formatParameters)
        {
            Ensure.NotNull(guard, "guard");
            Ensure.NotNullOrEmpty(argumentName, "argumentName");
            Ensure.NotNullOrEmpty(format, "format");
            return new ArgumentOutOfRangeException(argumentName, String.Format(format, formatParameters));
        }

        /// <summary>
        /// Creates exception <see cref="ArgumentOutOfRangeException"/> with message saying, that file on <paramref name="path"/> doesn't exist.
        /// </summary>
        /// <param name="guard">Exception helper.</param>
        /// <param name="path">Path to the not existing file.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <returns><see cref="ArgumentOutOfRangeException"/>.</returns>
        [DebuggerStepThrough]
        public static ArgumentOutOfRangeException ArgumentFileNotExist(this EnsureExceptionHelper guard, string path, string argumentName)
        {
            return guard.ArgumentOutOfRange(argumentName, "Path must point to an existing file, Path '{0}' doesn't exist.", path);
        }

        /// <summary>
        /// Creates exception <see cref="ArgumentOutOfRangeException"/> with message saying, that directory on <paramref name="path"/> doesn't exist.
        /// </summary>
        /// <param name="guard">Exception helper.</param>
        /// <param name="path">Path to the not existing directory.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <returns><see cref="ArgumentOutOfRangeException"/>.</returns>
        [DebuggerStepThrough]
        public static ArgumentOutOfRangeException ArgumentDirectoryNotExist(this EnsureExceptionHelper guard, string path, string argumentName)
        {
            return guard.ArgumentOutOfRange(argumentName, "Path must point to an existing directory, Path '{0}' doesn't exist.", path);
        }
    }
}
