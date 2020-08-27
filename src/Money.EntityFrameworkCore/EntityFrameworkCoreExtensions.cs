using Microsoft.Extensions.Configuration;
using Money.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore
{
    public static class EntityFrameworkCoreExtensions
    {
        public static void UseDbServer(this DbContextOptionsBuilder options, IConfiguration configuration, PathResolver pathResolver, string schema)
        {
            if (configuration.GetValue("Server", DbServer.Sqlite) == DbServer.SqlServer)
            {
                options.UseSqlServer(configuration.GetValue<string>("ConnectionString"), sql =>
                {
                    if (!String.IsNullOrEmpty(schema))
                        sql.MigrationsHistoryTable("__EFMigrationsHistory", schema);
                });
            }
            else
            {
                options.UseSqlite(pathResolver(configuration.GetValue<string>("ConnectionString")));
            }
        }
    }
}
