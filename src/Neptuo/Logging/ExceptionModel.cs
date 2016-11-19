using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging
{
    /// <summary>
    /// Describes exception with message.
    /// </summary>
    public class ExceptionModel
    {
        /// <summary>
        /// Custom message associated with exception.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Raised exception.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="message">Custom message associated with exception.</param>
        /// <param name="exception">Raised exception.</param>
        public ExceptionModel(string message, Exception exception)
        {
            Ensure.NotNullOrEmpty(message, "message");
            Ensure.NotNull(exception, "exception");
            Message = message;
            Exception = exception;
        }

        public override int GetHashCode()
        {
            return 7 ^ Message.GetHashCode() ^ Exception.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ExceptionModel other = obj as ExceptionModel;
            if (other == null)
                return false;

            if (other.Message != Message)
                return false;

            if (other.Exception != Exception)
                return false;

            return true;
        }
    }
}
