using Microsoft.EntityFrameworkCore;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.EntityFrameworkCore
{
    public class EventSourcingContext : Neptuo.Data.Entity.EventSourcingContext
    {
        private readonly SchemaOptions schema;

        public EventSourcingContext(DbContextOptions<EventSourcingContext> options, SchemaOptions<EventSourcingContext> schema)
            : base(options, false)
        {
            Ensure.NotNull(schema, "schema");
            this.schema = schema;

            InitializeSets();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (!String.IsNullOrEmpty(schema.Name))
            {
                modelBuilder.HasDefaultSchema(schema.Name);

                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                    entity.SetSchema(schema.Name);
            }
        }
    }
}
