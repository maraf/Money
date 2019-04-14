using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// An user profile information.
    /// </summary>
    public class ProfileModel
    {
        /// <summary>
        /// Gets an username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets an user's email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="userName">An username.</param>
        /// <param name="email">An user's email.</param>
        public ProfileModel(string userName, string email)
        {
            Ensure.NotNull(userName, "userName");
            UserName = userName;
            Email = email;
        }
    }
}
