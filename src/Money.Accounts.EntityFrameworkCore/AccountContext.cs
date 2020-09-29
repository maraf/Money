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

        public DbSet<UserPropertyKey> UserPropertyKeys { get; set; }
        public DbSet<UserPropertyValue> UserPropertyValues { get; set; }

        public AccountContext(DbContextOptions<AccountContext> options, SchemaOptions<AccountContext> schema)
            : base(options)
        {
            Ensure.NotNull(schema, "schema");
            this.schema = schema;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            modelBuilder.Entity<User>()
                .Property(b => b.CreatedAt)
                .HasDefaultValue(DateTime.MinValue);

            modelBuilder.Entity<UserPropertyKey>()
                .HasKey(p => p.Name);

            modelBuilder.Entity<UserPropertyKey>()
                .Property(p => p.Name)
                .HasMaxLength(256);

            modelBuilder.Entity<UserPropertyValue>()
                .HasOne(v => v.User)
                .WithMany(u => u.Properties)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPropertyValue>()
                .HasOne(v => v.Key)
                .WithMany(v => v.Values)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPropertyValue>()
                .HasKey(nameof(UserPropertyValue.UserId), nameof(UserPropertyValue.KeyName));

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
