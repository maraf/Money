using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Money.EntityFrameworkCore;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Money
{
    public class AccountContext : IdentityDbContext<User>
    {
        private readonly SchemaOptions schema;

        public DbSet<UserPropertyValue> UserProperties { get; set; }

        public AccountContext(DbContextOptions<AccountContext> options, SchemaOptions<AccountContext> schema)
            : base(options)
        {
            Ensure.NotNull(schema, "schema");
            this.schema = schema;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(b => b.CreatedAt)
                .HasDefaultValue(DateTime.MinValue);

            modelBuilder.Entity<UserPropertyValue>()
                .HasKey(p => new { p.UserId, p.Key });

            modelBuilder.Entity<UserPropertyValue>()
                .HasOne(p => p.User)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<UserPropertyValue>()
                .Property(p => p.Value)
                .HasMaxLength(1024);

            if (!String.IsNullOrEmpty(schema.Name))
            {
                modelBuilder.HasDefaultSchema(schema.Name);

                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                    entity.SetSchema(schema.Name);
            }
        }
    }
}
