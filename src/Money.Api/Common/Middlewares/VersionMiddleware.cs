using Microsoft.AspNetCore.Http;
using Money.Api;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Money.Common.Middlewares
{
    public class VersionMiddleware : IMiddleware
    {
        private readonly Version version;

        public VersionMiddleware(VersionMiddlewareOptions options)
        {
            Ensure.NotNull(options, "options");
            version = options.AppAssembly.GetName().Version;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (version != null)
                context.Response.Headers[VersionHeader.Name] = version.ToString(4);

            return next(context);
        }
    }
}
