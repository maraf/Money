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
        public DbSet<PushSubscription> PushSubscriptions { get; set; }
        public DbSet<UserNotificationSettings> UserNotificationSettings { get; set; }
        public DbSet<UserNotificationExpenseTemplateSettings> UserNotificationExpenseTemplateSettings { get; set; }
        public DbSet<ExpenseTemplateNotificationDispatch> ExpenseTemplateNotificationDispatches { get; set; }

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

            modelBuilder.Entity<PushSubscription>()
                .HasIndex(p => p.Endpoint)
                .IsUnique();

            modelBuilder.Entity<PushSubscription>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserNotificationSettings>()
                .HasKey(s => s.UserId);

            modelBuilder.Entity<UserNotificationSettings>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserNotificationExpenseTemplateSettings>()
                .HasKey(s => s.UserId);

            modelBuilder.Entity<UserNotificationExpenseTemplateSettings>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExpenseTemplateNotificationDispatch>()
                .HasIndex(d => new { d.UserId, d.ExpenseTemplateId, d.Date })
                .IsUnique();

            modelBuilder.Entity<ExpenseTemplateNotificationDispatch>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            if (!String.IsNullOrEmpty(schema.Name))
            {
                modelBuilder.HasDefaultSchema(schema.Name);

                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                    entity.SetSchema(schema.Name);
            }
        }
    }
}
