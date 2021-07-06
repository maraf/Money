using Microsoft.AspNetCore.Builder;
using Money.Common.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class ExceptionMiddlewareExtentions
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder) => builder.UseMiddleware<ExceptionMiddleware>();
    }
}
