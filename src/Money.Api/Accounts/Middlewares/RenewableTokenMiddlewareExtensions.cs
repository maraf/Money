using Money.Accounts.Middlewares;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class RenewableTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseRenewableToken(this IApplicationBuilder builder) => builder.UseMiddleware<RenewableTokenMiddleware>();
    }
}
