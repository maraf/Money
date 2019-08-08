using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Common.Diagnostics
{
    public static class ExceptionMiddlewareExceptions
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder) => builder.UseMiddleware<ExceptionMiddleware>();
    }
}
