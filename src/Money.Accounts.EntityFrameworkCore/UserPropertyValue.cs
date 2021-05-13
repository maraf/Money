using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    public class UserPropertyValue
    {
        public User User { get; set; }
        public string UserId { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
