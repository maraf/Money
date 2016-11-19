using Neptuo.Activators;
using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands.Handlers
{
    /// <summary>
    /// The base command handler for process root commands.
    /// </summary>
    /// <typeparam name="T">The type of the process root.</typeparam>
    public class ProcessRootCommandHandler<T>
        where T : ProcessRoot
    {
        private readonly AggregateRootCommandExecutor<T, IProcessRootRepository<T>> defaultExecutor;

        /// <summary>
        /// Creates new instance that uses <paramref name="repositoryFactory"/> for creating instances of repository.
        /// </summary>
        /// <param name="repositoryFactory">The factory for instances of the repository.</param>
        public ProcessRootCommandHandler(IFactory<IProcessRootRepository<T>> repositoryFactory)
        {
            defaultExecutor = new AggregateRootCommandExecutor<T, IProcessRootRepository<T>>(repositoryFactory, null, GetProcess, SaveProcess);
        }

        /// <summary>
        /// Executes <paramref name="handler"/> that creates new instance of process and saves it.
        /// Nothing about source command is saved.
        /// </summary>
        /// <param name="handler">The handler that creates new instance of process; when <c>null</c> is returned, nothing is saved.</param>
        protected Task Execute(Func<T> handler)
        {
            return defaultExecutor.Execute(handler);
        }

        /// <summary>
        /// Loads process by <paramref name="key"/> and executes <paramref name="handler"/> with it. Then the process is saved.
        /// Nothing about source command is saved.
        /// </summary>
        /// <param name="key">The key of the process to load.</param>
        /// <param name="handler">The handler method for modifying process.</param>
        protected Task Execute(IKey key, Action<T> handler)
        {
            return defaultExecutor.Execute(key, handler);
        }

        /// <summary>
        /// Loads aggregate root with the <paramref name="key"/> from the <paramref name="repository"/>.
        /// </summary>
        /// <param name="repository">The repository to load the process root from.</param>
        /// <param name="key">The key of the process root to load.</param>
        /// <returns>The loaded process root.</returns>
        protected virtual T GetProcess(IProcessRootRepository<T> repository, IKey key)
        {
            T aggregate = repository.Get(key);
            return aggregate;
        }

        /// <summary>
        /// Saves the <paramref name="process"/> root to the <paramref name="repository"/>.
        /// </summary>
        /// <param name="repository">The repository to save the process root to.</param>
        /// <param name="process">The process root to save.</param>
        protected virtual void SaveProcess(IProcessRootRepository<T> repository, T process, IKey commandKey)
        {
            repository.Save(process, commandKey);
        }

        /// <summary>
        /// Uses command executor that saved information about source command key.
        /// </summary>
        /// <param name="commandKey">The key of the command that initiates execute operations.</param>
        /// <returns>The instance of command executor associated with the command key.</returns>
        protected AggregateRootCommandExecutor<T, IProcessRootRepository<T>> WithCommand(IKey commandKey)
        {
            return defaultExecutor.WithCommand(commandKey);
        }
    }
}
