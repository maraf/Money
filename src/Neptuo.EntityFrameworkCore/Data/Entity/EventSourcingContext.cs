using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data.Entity
{
    /// <summary>
    /// The default implementation of <see cref="IEventContext"/>.
    /// </summary>
    public class EventSourcingContext : DbContext, IEventContext, ICommandContext
    {
        public DbSet<EventEntity> Events { get; private set; }
        public DbSet<UnPublishedEventEntity> UnPublishedEvents { get; private set; }

        public DbSet<CommandEntity> Commands { get; private set; }
        public DbSet<UnPublishedCommandEntity> UnPublishedCommands { get; private set; }

        public EventSourcingContext(DbContextOptions options, bool initializeSets = true)
            : base(options)
        {
            if (initializeSets)
                InitializeSets();
        }

        protected void InitializeSets()
        {
            Events = Set<EventEntity>();
            UnPublishedEvents = Set<UnPublishedEventEntity>();

            Commands = Set<CommandEntity>();
            UnPublishedCommands = Set<UnPublishedCommandEntity>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventEntity>()
                .ToTable("Event");

            modelBuilder.Entity<EventPublishedToHandlerEntity>()
                .ToTable("EventPublishedToHandler");

            modelBuilder.Entity<UnPublishedEventEntity>()
                .ToTable("UnPublishedEvent");

            modelBuilder.Entity<CommandEntity>()
                .ToTable("Command");

            modelBuilder.Entity<UnPublishedCommandEntity>()
                .ToTable("UnPublishedCommand");
        }

        public void Save()
        {
            base.SaveChanges();
        }

        public Task SaveAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}
