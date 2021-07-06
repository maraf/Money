using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Accounts.Models
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool IsAutoRenewable { get; set; }
    }
}
