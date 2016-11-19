using Neptuo.Data;
using Neptuo.Formatters;
using Neptuo.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    /// <summary>
    /// The factory for <see cref="PersistentCommandDispatcher"/> that shares threading and paralelism.
    /// </summary>
    public class PersistentCommandDispatcherBuilder
    {
        private TreeQueue queue;
        private TreeQueueThreadPool threadPool;

        private ICommandDistributor distributor;
        private ICommandPublishingStore store;
        private ISerializer formatter;
        private ISchedulingProvider schedulingProvider;

        private void EnsureInternals()
        {
            if (queue == null)
                queue = new TreeQueue();

            if (threadPool == null)
                threadPool = new TreeQueueThreadPool(queue);
        }

        /// <summary>
        /// Sets <paramref name="distributor"/> to be used for distributing commands on all newly created dispatchers.
        /// </summary>
        /// <param name="distributor">The command-to-the-queue distributor.</param>
        /// <returns>Self (for fluency).</returns>
        public PersistentCommandDispatcherBuilder UseCommandDistributor(ICommandDistributor distributor)
        {
            Ensure.NotNull(distributor, "distributor");
            this.distributor = distributor;
            return this;
        }

        /// <summary>
        /// Sets <paramref name="store"/> to be used for persisting commands on all newly created dispatchers.
        /// </summary>
        /// <param name="store">The publishing store for command persistent delivery (can be <c>null</c> for reseting to default).</param>
        /// <returns>Self (for fluency).</returns>
        public PersistentCommandDispatcherBuilder UseStore(ICommandPublishingStore store)
        {
            this.store = store;
            return this;
        }

        /// <summary>
        /// Sets <paramref name="formatter"/> to be used for serializing commands on all newly created dispatchers.
        /// </summary>
        /// <param name="formatter">The formatter for serializing commands.</param>
        /// <returns>Self (for fluency).</returns>
        public PersistentCommandDispatcherBuilder UseFormatter(ISerializer formatter)
        {
            Ensure.NotNull(formatter, "formatter");
            this.formatter = formatter;
            return this;
        }

        /// <summary>
        /// Sets <paramref name="schedulingProvider"/> to be used for delaying commands on all newly created dispatchers.
        /// </summary>
        /// <param name="schedulingProvider">The provider of a delay computation for delayed commands (can be <c>null</c> for reseting to default).</param>
        /// <returns>Self (for fluency).</returns>
        public PersistentCommandDispatcherBuilder UseSchedulingProvider(ISchedulingProvider schedulingProvider)
        {
            this.schedulingProvider = schedulingProvider;
            return this;
        }

        /// <summary>
        /// Creates new instance of <see cref="PersistentCommandDispatcher"/> based on passed components.
        /// If some of required dependencies are missing, the exception is thrown.
        /// </summary>
        /// <returns>New instance of <see cref="PersistentCommandDispatcher"/>.</returns>
        public PersistentCommandDispatcher Create()
        {
            Ensure.NotNull(distributor, "distributor");
            Ensure.NotNull(formatter, "formatter");
            Ensure.NotNull(schedulingProvider, "schedulingProvider");
            EnsureInternals();

            return new PersistentCommandDispatcher(
                queue,
                threadPool,
                distributor,
                store,
                formatter,
                schedulingProvider
            );
        }
    }
}
