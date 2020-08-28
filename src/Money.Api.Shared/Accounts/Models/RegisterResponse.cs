using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Accounts.Models
{
    public class RegisterResponse
    {
        public bool IsSuccess => ErrorMessages.Count == 0;

        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}
