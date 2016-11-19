using Neptuo.Activators;
using Neptuo.Models;
using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands.Handlers
{
    /// <summary>
    /// The helper class for execution commands, catching exceptions, passing source command key.
    /// </summary>
    /// <typeparam name="T">The type of the aggregate root.</typeparam>
    public class AggregateRootCommandExecutor<T, TRepository>
        where T : AggregateRoot
        where TRepository : IRepository<T, IKey>
    {
        private readonly IFactory<TRepository> repositoryFactory;
        private readonly IKey commandKey;
        private readonly Func<TRepository, IKey, T> getAggregate;
        private readonly Action<TRepository, T, IKey> saveAggregate;

        internal AggregateRootCommandExecutor(IFactory<TRepository> repositoryFactory, IKey commandKey, Func<TRepository, IKey, T> getAggregate, Action<TRepository, T, IKey> saveAggregate)
        {
            Ensure.NotNull(repositoryFactory, "repositoryFactory");
            Ensure.NotNull(getAggregate, "getAggregate");
            Ensure.NotNull(saveAggregate, "saveAggregate");
            this.repositoryFactory = repositoryFactory;
            this.commandKey = commandKey;
            this.getAggregate = getAggregate;
            this.saveAggregate = saveAggregate;
        }

        /// <summary>
        /// Executes <paramref name="handler"/> that creates new instance of aggregate and saves it.
        /// </summary>
        /// <param name="handler">The handler that creates new instance of aggregate; when <c>null</c> is returned, nothing is saved.</param>
        public Task Execute(Func<T> handler)
        {
            Ensure.NotNull(handler, "handler");

            try
            {
                T aggregate = handler();
                if (aggregate != null)
                {
                    TRepository repository = repositoryFactory.Create();
                    saveAggregate(repository, aggregate, commandKey);
                }
            }
            catch (AggregateRootException e)
            {
                if (e.Key == null)
                    e.Key = KeyFactory.Empty(typeof(T));

                if (commandKey != null)
                    e.CommandKey = commandKey;

                throw e;
            }

            return Async.CompletedTask;
        }

        /// <summary>
        /// Loads aggregate by <paramref name="key"/> and executes <paramref name="handler"/> with it. Then the aggregate is saved.
        /// </summary>
        /// <param name="key">The key of the aggregate to load.</param>
        /// <param name="handler">The handler method for modifying aggregate.</param>
        public Task Execute(IKey key, Action<T> handler)
        {
            Ensure.NotNull(key, "key");
            Ensure.NotNull(handler, "handler");

            TRepository repository = repositoryFactory.Create();
            T aggregate = getAggregate(repository, key);

            try
            {
                handler(aggregate);
                saveAggregate(repository, aggregate, commandKey);
            }
            catch (AggregateRootException e)
            {
                if (e.Key == null)
                    e.Key = key;

                if (commandKey != null)
                    e.CommandKey = commandKey;

                throw e;
            }

            return Async.CompletedTask;
        }

        internal AggregateRootCommandExecutor<T, TRepository> WithCommand(IKey commandKey)
        {
            return new AggregateRootCommandExecutor<T, TRepository>(repositoryFactory, commandKey, getAggregate, saveAggregate);
        }
    }
}
