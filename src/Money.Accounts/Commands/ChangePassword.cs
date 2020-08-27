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
    /// A command for changing user password.
    /// </summary>
    public class ChangePassword : Command
    {
        /// <summary>
        /// Gets a current password.
        /// </summary>
        public string Current { get; }

        /// <summary>
        /// Gets a new password.
        /// </summary>
        public string New { get; }

        /// <summary>
        /// Creates a new instance for changing password.
        /// </summary>
        /// <param name="current">A current password.</param>
        /// <param name="new">A new password.</param>
        public ChangePassword(string current, string @new)
        {
            Ensure.NotNull(current, "current");
            Ensure.NotNull(@new, "new");
            Current = current;
            New = @new;
        }
    }
}
