using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Money
{
    public class User : IdentityUser
    {
        public User()
        { }

        public User(string userName)
            : base(userName)
        { }
    }
}
