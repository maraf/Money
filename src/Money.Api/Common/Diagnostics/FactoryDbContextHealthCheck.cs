using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Neptuo;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Money.Common.Diagnostics
{
    public class FactoryDbContextHealthCheck<T> : IHealthCheck
        where T : DbContext
    {
        private readonly IFactory<T> dbContextFactory;

        public FactoryDbContextHealthCheck(IFactory<T> dbContextFactory)
        {
            Ensure.NotNull(dbContextFactory, "dbContextFactory");
            this.dbContextFactory = dbContextFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            DbContext dbContext = dbContextFactory.Create();
            if (await dbContext.Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Healthy();

            return HealthCheckResult.Unhealthy();
        }
    }
}
