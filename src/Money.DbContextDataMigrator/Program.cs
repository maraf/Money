using Microsoft.EntityFrameworkCore;
using Money.EntityFrameworkCore;
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

        private static Task CopyDbSetAsync<TContext, TEntity>(TContext sourceContext, TContext targetContext, Func<TContext, DbSet<TEntity>> dbSetGetter, Action<TEntity> entityHandler = null, params string[] includes)
            where TEntity : class
        {
            var source = dbSetGetter(sourceContext);
            var target = dbSetGetter(targetContext);
            return CopyDbSetAsync(source, target, entityHandler, includes);
        }

        private async static Task CopyDbSetAsync<T>(DbSet<T> source, DbSet<T> target, Action<T> entityHandler = null, params string[] includes)
            where T : class
        {
            IQueryable<T> query = source;
            foreach (string include in includes)
                query = query.Include(include);

            var entities = await query.ToListAsync();

            foreach (var entity in entities)
            {
                if (target.Contains(entity))
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
            using (var source = new AccountContext(new DbContextOptionsBuilder<AccountContext>().UseSqlite(sourceConnectionString).Options, new SchemaOptions<AccountContext>()))
            using (var target = new AccountContext(new DbContextOptionsBuilder<AccountContext>().UseSqlServer(targetConnectionString).Options, new SchemaOptions<AccountContext>() { Name = "Application" }))
            {
                await CopyDbSetAsync(source, target, c => c.Users);
                await CopyDbSetAsync(source, target, c => c.UserClaims);
                await CopyDbSetAsync(source, target, c => c.UserLogins);
                await CopyDbSetAsync(source, target, c => c.UserTokens);
                await CopyDbSetAsync(source, target, c => c.Roles);
                await CopyDbSetAsync(source, target, c => c.RoleClaims);
                await CopyDbSetAsync(source, target, c => c.UserRoles);

                await target.SaveChangesAsync();
            }
        }

        private async static Task MigrateEventSourcingAsync(string sourceConnectionString, string targetConnectionString)
        {
            using (var source = new EventSourcingContext(new DbContextOptionsBuilder<EventSourcingContext>().UseSqlite(sourceConnectionString).Options, new SchemaOptions<EventSourcingContext>()))
            using (var target = new EventSourcingContext(new DbContextOptionsBuilder<EventSourcingContext>().UseSqlServer(targetConnectionString).Options, new SchemaOptions<EventSourcingContext>() { Name = "EventSourcing" }))
            {
                await CopyDbSetAsync(source, target, c => c.Events);

                await target.SaveChangesAsync();
            }
        }

        private async static Task MigrateReadModelAsync(string sourceConnectionString, string targetConnectionString)
        {
            using (var source = new ReadModelContext(new DbContextOptionsBuilder<ReadModelContext>().UseSqlite(sourceConnectionString).Options, new SchemaOptions<ReadModelContext>()))
            using (var target = new ReadModelContext(new DbContextOptionsBuilder<ReadModelContext>().UseSqlServer(targetConnectionString).Options, new SchemaOptions<ReadModelContext>() { Name = "ReadModel" }))
            {
                await CopyDbSetAsync(source, target, c => c.Currencies);
                await CopyDbSetAsync(source, target, c => c.Categories);
                await CopyDbSetAsync(source, target, c => c.ExchangeRates);
                await CopyDbSetAsync(source, target, c => c.Outcomes);
                await CopyDbSetAsync(source, target, c => c.OutcomeCategories);

                await target.SaveChangesAsync();
            }
        }
    }
}
