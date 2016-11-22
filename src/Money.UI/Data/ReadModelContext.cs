using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Money.Data
{
    internal class ReadModelContext : DbContext
    {
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<OutcomeEntity> Outcomes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=ReadModel.db");
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
        }
    }
}
