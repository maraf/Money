using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Money.EntityFrameworkCore;
using Money.EntityFrameworkCore.Migrations;
using Neptuo;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                .AddSingleton(provider => Factory.Getter(() => provider.CreateScope().ServiceProvider.GetRequiredService<TContext>()));

            return services;
        }
    }
}
