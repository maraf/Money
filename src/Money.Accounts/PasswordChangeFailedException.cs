using Neptuo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// An exception raised when user password change fails.
    /// </summary>
    public class PasswordChangeFailedException : AggregateRootException
    {
        /// <summary>
        /// Gets a user error message.
        /// </summary>
        public string ErrorDescription { get; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="errorDescriptions">A user error message.</param>
        public PasswordChangeFailedException(string errorDescription)
        {
            ErrorDescription = errorDescription;
        }
    }
}
