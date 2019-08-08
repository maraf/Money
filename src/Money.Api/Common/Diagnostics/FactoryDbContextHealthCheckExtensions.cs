using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Common.Diagnostics
{
    public static class FactoryDbContextHealthCheckExtensions
    {
        public static IHealthChecksBuilder AddFactoryDbContextCheck<T>(this IHealthChecksBuilder builder)
            where T: DbContext
        {
            return builder.AddCheck<FactoryDbContextHealthCheck<T>>(typeof(T).Name);
        }
    }
}
