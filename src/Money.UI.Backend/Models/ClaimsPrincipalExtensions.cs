using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsDemo(this ClaimsPrincipal user) => user.Identity.Name == "demo";
    }
}
