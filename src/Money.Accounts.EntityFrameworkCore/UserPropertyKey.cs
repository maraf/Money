using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    public class UserPropertyKey
    {
        public string Name { get; set; }

        public ICollection<UserPropertyValue> Values { get; set; }
    }
}
