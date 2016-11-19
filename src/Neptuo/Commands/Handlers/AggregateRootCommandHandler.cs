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
    /// The base command handler for aggregate root commands.
    /// </summary>
    /// <typeparam name="T">The type of the aggregate root.</typeparam>
    public abstract class AggregateRootCommandHandler<T>
        where T : AggregateRoot
    {
        private readonly AggregateRootCommandExecutor<T, IRepository<T, IKey>> defaultExecutor;

        /// <summary>
        /// Creates new instance that uses <paramref name="repositoryFactory"/> for creating instances of repository.
        /// </summary>
        /// <param name="repositoryFactory">The factory for instances of the repository.</param>
        public AggregateRootCommandHandler(IFactory<IRepository<T, IKey>> repositoryFactory)
        {
            defaultExecutor = new AggregateRootCommandExecutor<T, IRepository<T, IKey>>(repositoryFactory, null, GetAggregate, SaveAggregate);
        }

        /// <summary>
        /// Executes <paramref name="handler"/> that creates new instance of aggregate and saves it.
        /// Nothing about source command is saved.
        /// </summary>
        /// <param name="handler">The handler that creates new instance of aggregate; when <c>null</c> is returned, nothing is saved.</param>
        protected Task Execute(Func<T> handler)
        {
            return defaultExecutor.Execute(handler);
        }

        /// <summary>
        /// Loads aggregate by <paramref name="key"/> and executes <paramref name="handler"/> with it. Then the aggregate is saved.
        /// Nothing about source command is saved.
        /// </summary>
        /// <param name="key">The key of the aggregate to load.</param>
        /// <param name="handler">The handler method for modifying aggregate.</param>
        protected Task Execute(IKey key, Action<T> handler)
        {
            return defaultExecutor.Execute(key, handler);
        }
        
        /// <summary>
        /// Loads aggregate root with the <paramref name="key"/> from the <paramref name="repository"/>.
        /// </summary>
        /// <param name="repository">The repository to load the aggregate root from.</param>
        /// <param name="key">The key of the aggregate root to load.</param>
        /// <returns>The loaded aggregate root.</returns>
        protected virtual T GetAggregate(IRepository<T, IKey> repository, IKey key)
        {
            T aggregate = repository.Get(key);
            return aggregate;
        }

        /// <summary>
        /// Saves the <paramref name="aggregate"/> root to the <paramref name="repository"/>.
        /// </summary>
        /// <param name="repository">The repository to save the aggreate root to.</param>
        /// <param name="aggregate">The aggregate root to save.</param>
        /// <param name="commandKey">The key of the command (if specified; or <c>null</c>).</param>
        protected virtual void SaveAggregate(IRepository<T, IKey> repository, T aggregate, IKey commandKey)
        {
            repository.Save(aggregate);
        }

        /// <summary>
        /// Uses command executor that saved information about source command key.
        /// </summary>
        /// <param name="commandKey">The key of the command that initiates execute operations.</param>
        /// <returns>The instance of command executor associated with the command key.</returns>
        protected AggregateRootCommandExecutor<T, IRepository<T, IKey>> WithCommand(IKey commandKey)
        {
            return defaultExecutor.WithCommand(commandKey);
        }
    }
}
