using Microsoft.Extensions.DependencyInjection;
using Money.Common.Middlewares;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class VersionMiddlewareExtensions
    {
        public static IServiceCollection AddVersionHeader(this IServiceCollection services, Assembly appAssembly = null)
        {
            var options = new VersionMiddlewareOptions();
            options.AppAssembly = appAssembly ?? Assembly.GetCallingAssembly();

            return services
                .AddSingleton(options)
                .AddTransient<VersionMiddleware>();
        }

        public static IApplicationBuilder UseVersionHeader(this IApplicationBuilder builder) => builder.UseMiddleware<VersionMiddleware>();
    }
}
