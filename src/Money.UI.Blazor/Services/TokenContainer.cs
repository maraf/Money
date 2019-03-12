using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class TokenContainer
    {
        public string Value { get; set; }

        public bool HasValue => !String.IsNullOrEmpty(Value);
    }
}
