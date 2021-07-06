using Microsoft.EntityFrameworkCore;
using Money.EntityFrameworkCore;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class ReadModelContext : DbContext
    {
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<OutcomeCategoryEntity> OutcomeCategories { get; set; }
        public DbSet<OutcomeEntity> Outcomes { get; set; }
        public DbSet<ExpenseTemplateEntity> ExpenseTemplates { get; set; }
        public DbSet<IncomeEntity> Incomes { get; set; }
        public DbSet<CurrencyEntity> Currencies { get; set; }
        public DbSet<CurrencyExchangeRateEntity> ExchangeRates { get; set; }

        private readonly SchemaOptions schema;

        public ReadModelContext(DbContextOptions<ReadModelContext> options, SchemaOptions<ReadModelContext> schema)
            : base(options)
        {
            Ensure.NotNull(schema, "schema");
            this.schema = schema;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OutcomeCategoryEntity>()
                .HasKey(t => new { t.OutcomeId, t.CategoryId });

            modelBuilder.Entity<OutcomeCategoryEntity>()
                .HasOne(pt => pt.Outcome)
                .WithMany(p => p.Categories)
                .HasForeignKey(pt => pt.OutcomeId);

            modelBuilder.Entity<OutcomeCategoryEntity>()
                .HasOne(pt => pt.Category)
                .WithMany(t => t.Outcomes)
                .HasForeignKey(pt => pt.CategoryId);

            modelBuilder.Entity<CurrencyEntity>()
                .HasKey(c => new { c.UserId, c.UniqueCode });

            modelBuilder.Entity<CurrencyExchangeRateEntity>()
                .HasKey(e => e.Id);

            if (!String.IsNullOrEmpty(schema.Name))
            {
                modelBuilder.HasDefaultSchema(schema.Name);

                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                    entity.SetSchema(schema.Name);
            }
        }
    }
}
