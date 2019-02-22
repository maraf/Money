using Neptuo;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    /// <summary>
    /// A command for changing user's email.
    /// </summary>
    public class ChangeEmail : Command
    {
        /// <summary>
        /// Gets an user's email.
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="email">An user's email.</param>
        public ChangeEmail(string email)
        {
            Ensure.NotNull(email, "email");
            Email = email;
        }
    }
}
