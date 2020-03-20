using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    partial class ApiAuthenticationStateProvider
    {
        public interface ITokenValidator
        {
            Task<bool> ValidateAsync(string token);
        }
    }
}
