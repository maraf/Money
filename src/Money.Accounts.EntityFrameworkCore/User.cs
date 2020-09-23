using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Money
{
    public class User : IdentityUser
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? LastSignedAt { get; set; }

        public ICollection<UserPropertyValue> Properties { get; set; }

        public User()
        { }

        public User(string userName)
            : base(userName)
        { }

        public User(string userName, DateTime createdAt)
            : this(userName)
        {
            CreatedAt = createdAt;
        }
    }
}
