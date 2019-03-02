using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Money.Users.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        { }

        public ApplicationUser(string userName)
            : base(userName)
        { }
    }
}
