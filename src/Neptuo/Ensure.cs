using Neptuo.Exceptions.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// Helper for throwing exceptions.
    /// </summary>
    public static class Ensure
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if <paramref name="argument"/> is <code>null</code>.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void NotNull(object argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if <paramref name="argument"/> is <code>null</code>.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <param name="message">Text description.</param>
        [DebuggerStepThrough]
        public static void NotNull(object argument, string argumentName, string message)
        {
            if (argument == null)
                throw Exception.Argument(message, argumentName);
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.
        /// Throws <see cref="ArgumentException"/> if <paramref name="argument"/> is equal to <code>String.Empty</code>.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void NotNullOrEmpty(string argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);

            if (String.IsNullOrEmpty(argument))
                throw Exception.Argument("Passed argument can't be empty string.", argumentName);
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is lower or equal to zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void Positive(int argument, string argumentName)
        {
            if (argument <= 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be positive (> 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is lower or equal to zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void Positive(long argument, string argumentName)
        {
            if (argument <= 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be positive (> 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is lower or equal to zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void Positive(double argument, string argumentName)
        {
            if (argument <= 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be positive (> 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is lower or equal to zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void Positive(decimal argument, string argumentName)
        {
            if (argument <= 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be positive (> 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is lower than zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void PositiveOrZero(int argument, string argumentName)
        {
            if (argument < 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be positive or zero (>= 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is lower than zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void PositiveOrZero(long argument, string argumentName)
        {
            if (argument < 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be positive or zero (>= 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is lower than zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void PositiveOrZero(double argument, string argumentName)
        {
            if (argument < 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be positive or zero (>= 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is lower than zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void PositiveOrZero(decimal argument, string argumentName)
        {
            if (argument < 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be positive or zero (>= 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is greater or equal to zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void Negative(int argument, string argumentName)
        {
            if (argument >= 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be negative (< 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is greater or equal to zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void Negative(long argument, string argumentName)
        {
            if (argument >= 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be negative (< 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is greater or equal to zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void Negative(double argument, string argumentName)
        {
            if (argument >= 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be negative (< 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is greater or equal to zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void Negative(decimal argument, string argumentName)
        {
            if (argument >= 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be negative (< 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is greater than zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void NegativeOrZero(int argument, string argumentName)
        {
            if (argument > 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be negative or zero (<= 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is greater than zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void NegativeOrZero(long argument, string argumentName)
        {
            if (argument > 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be negative or zero (<= 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is greater than zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void NegativeOrZero(double argument, string argumentName)
        {
            if (argument > 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be negative or zero (<= 0).");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argument"/> is greater than zero.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void NegativeOrZero(decimal argument, string argumentName)
        {
            if (argument > 0)
                throw Exception.ArgumentOutOfRange(argumentName, "Argument must be negative or zero (<= 0).");
        }

        /// <summary>
        /// Throws <see cref="ObjectDisposedException"/> if <paramref name="argument"/> is already disposed.
        /// </summary>
        /// <param name="argument">Argument to test.</param>
        /// <param name="argumentName">Argument name.</param>
        [DebuggerStepThrough]
        public static void NotDisposed(IDisposable argument, string argumentName)
        {
            if (argument.IsDisposed)
                throw new ObjectDisposedException(argumentName);
        }

        /// <summary>
        /// Helper for throwing exceptions.
        /// </summary>
        public static readonly EnsureExceptionHelper Exception = new EnsureExceptionHelper();

        /// <summary>
        /// Helper for custom conditions.
        /// </summary>
        public static readonly EnsureConditionHelper Condition = new EnsureConditionHelper();
    }
}
