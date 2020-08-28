using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Money;
using Money.EntityFrameworkCore;
using Money.EntityFrameworkCore.Migrations;
using Neptuo;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbSchema<TContext>(this IServiceCollection services, string schemaName) where TContext : DbContext
            => AddDbSchema(services, new SchemaOptions<TContext>() { Name = schemaName });

        public static IServiceCollection AddDbSchema<TContext>(this IServiceCollection services, SchemaOptions<TContext> schema)
            where TContext : DbContext
        {
            Ensure.NotNull(services, "services");
            Ensure.NotNull(schema, "schema");
            return services.AddSingleton(MigrationWithSchema.SetSchema(schema));
        }

        public static IServiceCollection AddDbContextWithSchema<TContext>(this IServiceCollection services, IConfiguration configuration, PathResolver pathResolver)
            where TContext : DbContext
        {
            Ensure.NotNull(services, "services");

            var schemaName = configuration.GetValue<string>("Schema");
            var schema = new SchemaOptions<TContext>() { Name = schemaName };

            services
                .AddDbSchema(schema)
                .AddDbContext<TContext>(options => options.UseDbServer(configuration, pathResolver, schema.Name))
                .AddSingleton<IFactory<TContext>, DbContextFactory<TContext>>();

            return services;
        }

        private class DbContextFactory<TContext> : IFactory<TContext>
            where TContext : DbContext
        {
            private readonly IServiceProvider provider;
            private object resolveLock = new object();

            private ConstructorInfo ctor;
            private DbContextOptions<TContext> dbContextOptions;
            private SchemaOptions<TContext> schemaOptions;

            public DbContextFactory(IServiceProvider provider)
            {
                this.provider = provider;
            }

            public TContext Create()
            {
                if (ctor == null)
                {
                    lock (resolveLock)
                    {
                        if (ctor == null)
                            ResolveDependencies();
                    }
                }

                return (TContext)ctor.Invoke(new object[] { dbContextOptions, schemaOptions });
            }

            private void ResolveDependencies()
            {
                dbContextOptions = provider.GetRequiredService<DbContextOptions<TContext>>();
                schemaOptions = provider.GetRequiredService<SchemaOptions<TContext>>();
                ctor = typeof(TContext).GetConstructor(new[] { typeof(DbContextOptions<TContext>), typeof(SchemaOptions<TContext>) });
            }
        }
    }
}
