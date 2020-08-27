using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.EntityFrameworkCore.Migrations
{
    public abstract class MigrationWithSchema : Migration
    {
        private static readonly Dictionary<Type, SchemaOptions> schema = new Dictionary<Type, SchemaOptions>();

        protected private SchemaOptions<TContext> GetSchema<TContext>()
            where TContext : DbContext
        {
            if (MigrationWithSchema.schema.TryGetValue(typeof(TContext), out var schema))
                return (SchemaOptions<TContext>)schema;

            return SchemaOptions<TContext>.Default;
        }

        public static SchemaOptions<TContext> SetSchema<TContext>(SchemaOptions<TContext> schema)
            where TContext : DbContext
        {
            Ensure.NotNull(schema, "schema");
            MigrationWithSchema.schema[typeof(TContext)] = schema;
            return schema;
        }
    }

    public abstract class MigrationWithSchema<TContext> : MigrationWithSchema
        where TContext : DbContext
    {
        protected SchemaOptions<TContext> Schema => GetSchema<TContext>();
    }
}
