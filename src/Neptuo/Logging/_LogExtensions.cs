using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging
{
    /// <summary>
    /// Common extensions for <see cref="ILogSerializer"/>.
    /// </summary>
    public static class _LogExtensions
    {
        /// <summary>
        /// Returns <c>true</c> when <see cref="LogLevel.Debug"/> is enabled in <paramref name="log"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns><c>true</c> when <see cref="LogLevel.Debug"/> is enabled in <paramref name="log"/>.</returns>
        public static bool IsDebugEnabled(this ILog log)
        {
            Ensure.NotNull(log, "log");
            return log.IsLevelEnabled(LogLevel.Debug);
        }

        /// <summary>
        /// Returns <c>true</c> when <see cref="LogLevel.Info"/> is enabled in <paramref name="log"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns><c>true</c> when <see cref="LogLevel.Info"/> is enabled in <paramref name="log"/>.</returns>
        public static bool IsInfoEnabled(this ILog log)
        {
            Ensure.NotNull(log, "log");
            return log.IsLevelEnabled(LogLevel.Info);
        }

        /// <summary>
        /// Returns <c>true</c> when <see cref="LogLevel.Warning"/> is enabled in <paramref name="log"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns><c>true</c> when <see cref="LogLevel.Warning"/> is enabled in <paramref name="log"/>.</returns>
        public static bool IsWarnEnabled(this ILog log)
        {
            Ensure.NotNull(log, "log");
            return log.IsLevelEnabled(LogLevel.Warning);
        }

        /// <summary>
        /// Returns <c>true</c> when <see cref="LogLevel.Error"/> is enabled in <paramref name="log"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns><c>true</c> when <see cref="LogLevel.Error"/> is enabled in <paramref name="log"/>.</returns>
        public static bool IsErrorEnabled(this ILog log)
        {
            Ensure.NotNull(log, "log");
            return log.IsLevelEnabled(LogLevel.Error);
        }

        /// <summary>
        /// Returns <c>true</c> when <see cref="LogLevel.Fatal"/> is enabled in <paramref name="log"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns><c>true</c> when <see cref="LogLevel.Fatal"/> is enabled in <paramref name="log"/>.</returns>
        public static bool IsFatalEnabled(this ILog log)
        {
            Ensure.NotNull(log, "log");
            return log.IsLevelEnabled(LogLevel.Fatal);
        }


        /// <summary>
        /// Logs <paramref name="message"/> to the <paramref name="log"/> at <see cref="LogLevel.Debug"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">Log message.</param>
        public static void Debug(this ILog log, string message)
        {
            Ensure.NotNull(log, "log");
            log.Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Logs <paramref name="messageFormat"/> using <paramref name="parameters" /> to the <paramref name="log"/> at <see cref="LogLevel.Debug"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">Log message format string.</param>
        /// <param name="parameters">Parameters for <paramref name="messageFormat"/>.</param>
        public static void Debug(this ILog log, string messageFormat, params object[] parameters)
        {
            Debug(log, String.Format(messageFormat, parameters));
        }


        /// <summary>
        /// Logs <paramref name="message"/> to the <paramref name="log"/> at <see cref="LogLevel.Info"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">Log message.</param>
        public static void Info(this ILog log, string message)
        {
            Ensure.NotNull(log, "log");
            log.Log(LogLevel.Info, message);
        }

        /// <summary>
        /// Logs <paramref name="messageFormat"/> using <paramref name="parameters" /> to the <paramref name="log"/> at <see cref="LogLevel.Info"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">Log message format string.</param>
        /// <param name="parameters">Parameters for <paramref name="messageFormat"/>.</param>
        public static void Info(this ILog log, string messageFormat, params object[] parameters)
        {
            Info(log, String.Format(messageFormat, parameters));
        }


        /// <summary>
        /// Logs <paramref name="message"/> to the <paramref name="log"/> at <see cref="LogLevel.Warning"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">Log message.</param>
        public static void Warning(this ILog log, string message)
        {
            Ensure.NotNull(log, "log");
            log.Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Logs <paramref name="messageFormat"/> using <paramref name="parameters" /> to the <paramref name="log"/> at <see cref="LogLevel.Warning"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">Log message format string.</param>
        /// <param name="parameters">Parameters for <paramref name="messageFormat"/>.</param>
        public static void Warning(this ILog log, string messageFormat, params object[] parameters)
        {
            Warning(log, String.Format(messageFormat, parameters));
        }


        /// <summary>
        /// Logs <paramref name="message"/> to the <paramref name="log"/> at <see cref="LogLevel.Error"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">Log message.</param>
        public static void Error(this ILog log, string message)
        {
            Ensure.NotNull(log, "log");
            log.Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Logs <paramref name="messageFormat"/> using <paramref name="parameters" /> to the <paramref name="log"/> at <see cref="LogLevel.Error"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">Log message format string.</param>
        /// <param name="parameters">Parameters for <paramref name="messageFormat"/>.</param>
        public static void Error(this ILog log, string messageFormat, params object[] parameters)
        {
            Error(log, String.Format(messageFormat, parameters));
        }

        /// <summary>
        /// Logs <paramref name="exception"/> to the <paramref name="log"/> at <see cref="LogLevel.Error"/>.
        /// If <paramref name="message" /> is specified (not null or empty), logs <see cref="ExceptionModel"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">Raised exception.</param>
        /// <param name="message">Custom message associated with <paramref name="exception"/>.</param>
        public static void Error(this ILog log, Exception exception, string message = null)
        {
            Ensure.NotNull(log, "log");
            if (String.IsNullOrEmpty(message))
                log.Log(LogLevel.Error, exception);
            else
                log.Log(LogLevel.Error, new ExceptionModel(message, exception));
        }

        /// <summary>
        /// Logs <paramref name="exception"/> to the <paramref name="log"/> at <see cref="LogLevel.Error"/>.
        /// If <paramref name="message" /> is specified (not null or empty), logs <see cref="ExceptionModel"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">Raised exception.</param>
        /// <param name="messageFormat">Custom message format associated with <paramref name="exception"/>.</param>
        /// <param name="parameters">Parameters for <paramref name="messageFormat"/>.</param>
        public static void Error(this ILog log, Exception exception, string messageFormat, params object[] parameters)
        {
            Ensure.NotNull(log, "log");
            if (String.IsNullOrEmpty(messageFormat))
                log.Log(LogLevel.Error, exception);
            else
                log.Log(LogLevel.Error, new ExceptionModel(String.Format(messageFormat, parameters), exception));
        }


        /// <summary>
        /// Logs <paramref name="message"/> to the <paramref name="log"/> at <see cref="LogLevel.Fatal"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">Log message.</param>
        public static void Fatal(this ILog log, string message)
        {
            Ensure.NotNull(log, "log");
            log.Log(LogLevel.Fatal, message);
        }

        /// <summary>
        /// Logs <paramref name="messageFormat"/> using <paramref name="parameters" /> to the <paramref name="log"/> at <see cref="LogLevel.Fatal"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">Log message format string.</param>
        /// <param name="parameters">Parameters for <paramref name="messageFormat"/>.</param>
        public static void Fatal(this ILog log, string messageFormat, params object[] parameters)
        {
            Fatal(log, String.Format(messageFormat, parameters));
        }

        /// <summary>
        /// Logs <paramref name="exception"/> to the <paramref name="log"/> at <see cref="LogLevel.Fatal"/>.
        /// If <paramref name="message" /> is specified (not null or empty), logs <see cref="ExceptionModel"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">Raised exception.</param>
        /// <param name="message">Custom message associated with <paramref name="exception"/>.</param>
        public static void Fatal(this ILog log, Exception exception, string message = null)
        {
            Ensure.NotNull(log, "log");
            if (String.IsNullOrEmpty(message))
                log.Log(LogLevel.Fatal, exception);
            else
                log.Log(LogLevel.Fatal, new ExceptionModel(message, exception));
        }

        /// <summary>
        /// Logs <paramref name="exception"/> to the <paramref name="log"/> at <see cref="LogLevel.Fatal"/>.
        /// If <paramref name="message" /> is specified (not null or empty), logs <see cref="ExceptionModel"/>.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">Raised exception.</param>
        /// <param name="messageFormat">Custom message format associated with <paramref name="exception"/>.</param>
        /// <param name="parameters">Parameters for <paramref name="messageFormat"/>.</param>
        public static void Fatal(this ILog log, Exception exception, string messageFormat, params object[] parameters)
        {
            Ensure.NotNull(log, "log");
            if (String.IsNullOrEmpty(messageFormat))
                log.Log(LogLevel.Fatal, exception);
            else
                log.Log(LogLevel.Fatal, new ExceptionModel(String.Format(messageFormat, parameters), exception));
        }
    }
}