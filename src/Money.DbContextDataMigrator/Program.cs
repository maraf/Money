using Microsoft.EntityFrameworkCore;
using Money.EntityFrameworkCore;
using Money.EntityFrameworkCore.Migrations;
using Money.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Money
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string accountsSource = "";
            string accountsTarget = "";
            string eventSourcingSource = "";
            string eventSourcingTarget = "";
            string readModelSource = "";
            string readModelTarget = "";

            await MigrateApplicationAsync(accountsSource, accountsTarget);
            await MigrateEventSourcingAsync(eventSourcingSource, eventSourcingTarget);
            await MigrateReadModelAsync(readModelSource, readModelTarget);
        }

        private static Task CopyDbSetAsync<TContext, TEntity>(TContext sourceContext, TContext targetContext, Func<TContext, DbSet<TEntity>> dbSetGetter, Action<TEntity> entityHandler = null, Func<TEntity, Task<bool>> containsHandler = null, params string[] includes)
            where TEntity : class
        {
            var source = dbSetGetter(sourceContext);
            var target = dbSetGetter(targetContext);
            return CopyDbSetAsync(source, target, entityHandler, containsHandler, includes);
        }

        private async static Task CopyDbSetAsync<T>(DbSet<T> source, DbSet<T> target, Action<T> entityHandler = null, Func<T, Task<bool>> containsHandler = null, params string[] includes)
            where T : class
        {
            IQueryable<T> query = source;
            foreach (string include in includes)
                query = query.Include(include);

            var entities = await query.ToListAsync();
            Console.WriteLine($"Processing {entities.Count} entities of type {typeof(T).Name}.");

            foreach (var entity in entities)
            {
                bool isContained;
                if (containsHandler != null)
                    isContained = await containsHandler(entity);
                else
                    isContained = target.Contains(entity);

                if (isContained)
                    target.Update(entity);
                else
                    target.Add(entity);
            }

            if (entityHandler != null)
            {
                foreach (var entity in entities)
                    entityHandler(entity);
            }
        }

        private async static Task MigrateApplicationAsync(string sourceConnectionString, string targetConnectionString)
        {
            Console.WriteLine("Migrate Application.");

            SchemaOptions<AccountContext> sourceSchema = new SchemaOptions<AccountContext>();
            SchemaOptions<AccountContext> targetSchema = new SchemaOptions<AccountContext>() { Name = "Application" };
            MigrationWithSchema.SetSchema(sourceSchema);
            MigrationWithSchema.SetSchema(targetSchema);

            using (var source = new AccountContext(new DbContextOptionsBuilder<AccountContext>().UseSqlite(sourceConnectionString).Options, sourceSchema))
            using (var target = new AccountContext(new DbContextOptionsBuilder<AccountContext>().UseSqlServer(targetConnectionString).Options, targetSchema))
            {
                Console.WriteLine("Migrate schema.");
                await target.Database.MigrateAsync();

                Console.WriteLine("Migrate data.");
                await CopyDbSetAsync(source, target, c => c.Users);
                await CopyDbSetAsync(source, target, c => c.UserClaims);
                await CopyDbSetAsync(source, target, c => c.UserLogins);
                await CopyDbSetAsync(source, target, c => c.UserTokens);
                await CopyDbSetAsync(source, target, c => c.Roles);
                await CopyDbSetAsync(source, target, c => c.RoleClaims);
                await CopyDbSetAsync(source, target, c => c.UserRoles);

                Console.WriteLine("Save changes.");
                await target.SaveChangesAsync();

                Console.WriteLine("Completed.");
            }
        }

        private async static Task MigrateEventSourcingAsync(string sourceConnectionString, string targetConnectionString)
        {
            Console.WriteLine("Migrate EventSourcing.");

            SchemaOptions<EventSourcingContext> sourceSchema = new SchemaOptions<EventSourcingContext>();
            SchemaOptions<EventSourcingContext> targetSchema = new SchemaOptions<EventSourcingContext>() { Name = "EventSourcing" };
            MigrationWithSchema.SetSchema(sourceSchema);
            MigrationWithSchema.SetSchema(targetSchema);

            using (var source = new EventSourcingContext(new DbContextOptionsBuilder<EventSourcingContext>().UseSqlite(sourceConnectionString).Options, sourceSchema))
            using (var target = new EventSourcingContext(new DbContextOptionsBuilder<EventSourcingContext>().UseSqlServer(targetConnectionString).Options, targetSchema))
            {
                Console.WriteLine("Migrate schema.");
                await target.Database.MigrateAsync();

                target.Database.OpenConnection();
                await target.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [EventSourcing].[Event] ON;");

                Console.WriteLine("Migrate data.");
                await CopyDbSetAsync(source, target, c => c.Events);

                Console.WriteLine("Save changes.");
                await target.SaveChangesAsync();

                await target.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [EventSourcing].[Event] OFF;");
                target.Database.CloseConnection();

                Console.WriteLine("Completed.");
            }
        }

        private async static Task MigrateReadModelAsync(string sourceConnectionString, string targetConnectionString)
        {
            Console.WriteLine("Migrate ReadModel.");

            SchemaOptions<ReadModelContext> sourceSchema = new SchemaOptions<ReadModelContext>();
            SchemaOptions<ReadModelContext> targetSchema = new SchemaOptions<ReadModelContext>() { Name = "ReadModel" };
            MigrationWithSchema.SetSchema(sourceSchema);
            MigrationWithSchema.SetSchema(targetSchema);

            using (var source = new ReadModelContext(new DbContextOptionsBuilder<ReadModelContext>().UseSqlite(sourceConnectionString).Options, sourceSchema))
            using (var target = new ReadModelContext(new DbContextOptionsBuilder<ReadModelContext>().UseSqlServer(targetConnectionString).Options, targetSchema))
            {
                Console.WriteLine("Migrate schema.");
                await target.Database.MigrateAsync();

                target.Database.OpenConnection();
                await target.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [ReadModel].[ExchangeRates] ON;");

                Console.WriteLine("Migrate data.");
                await CopyDbSetAsync(source, target, c => c.Currencies, containsHandler: e => target.Currencies.AnyAsync(c => c.UserId == e.UserId && c.UniqueCode == e.UniqueCode));
                await CopyDbSetAsync(source, target, c => c.Categories);
                await CopyDbSetAsync(source, target, c => c.ExchangeRates);
                await CopyDbSetAsync(source, target, c => c.Outcomes);
                await CopyDbSetAsync(source, target, c => c.OutcomeCategories, containsHandler: e => target.OutcomeCategories.AnyAsync(ec => e.CategoryId == ec.CategoryId && e.OutcomeId == ec.OutcomeId));

                Console.WriteLine("Save changes.");
                await target.SaveChangesAsync();

                await target.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [ReadModel].[ExchangeRates] OFF;");
                target.Database.CloseConnection();

                Console.WriteLine("Completed.");
            }
        }
    }
}
