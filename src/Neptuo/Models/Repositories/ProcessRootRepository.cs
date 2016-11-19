using Neptuo.Activators;
using Neptuo.Commands;
using Neptuo.Data;
using Neptuo.Events;
using Neptuo.Formatters;
using Neptuo.Internals;
using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using Neptuo.Models.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Repositories
{
    /// <summary>
    /// The implementation of EventSourcing ProcessRoot repository.
    /// </summary>
    /// <typeparam name="T">The type of the process root.</typeparam>
    public class ProcessRootRepository<T> : AggregateRootRepository<T>, IProcessRootRepository<T>
        where T : ProcessRoot
    {
        private readonly ISerializer commandFormatter;
        private readonly ICommandStore commandStore;
        private readonly ICommandDispatcher commandDispatcher;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="eventStore">The underlaying event store.</param>
        /// <param name="commandStore">The underlaying command store.</param>
        /// <param name="eventFormatter">The formatter for serializing event payloads.</param>
        /// <param name="commandFormatter">The formatter for serializing commands.</param>
        /// <param name="factory">The process root factory.</param>
        /// <param name="eventDispatcher">The dispatcher for newly created events in the processes.</param>
        /// <param name="commandDispatcher">The dispatcher for newly created commands in the processes.</param>
        /// <param name="snapshotProvider">The snapshot provider.</param>
        /// <param name="snapshotStore">The store for snapshots.</param>
        public ProcessRootRepository(IEventStore eventStore, ICommandStore commandStore, IFormatter eventFormatter, ISerializer commandFormatter, 
            IAggregateRootFactory<T> factory, IEventDispatcher eventDispatcher, ICommandDispatcher commandDispatcher, ISnapshotProvider snapshotProvider, ISnapshotStore snapshotStore)
            : base(eventStore, eventFormatter, factory, eventDispatcher, snapshotProvider, snapshotStore)
        {
            Ensure.NotNull(commandStore, "commandStore");
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(commandFormatter, "commandFormatter");
            this.commandStore = commandStore;
            this.commandFormatter = commandFormatter;
            this.commandDispatcher = commandDispatcher;
        }

        public override void Save(T model)
        {
            Save(model, null);
        }
        
        public void Save(T model, IKey sourceCommandKey)
        {
            base.Save(model);

            IEnumerable<Envelope<ICommand>> commands = model.Commands;
            if (commands.Any())
            {
                IEnumerable<CommandModel> commandModels = commands.Select(c =>
                {
                    if (sourceCommandKey != null)
                        c.AddSourceCommandKey(sourceCommandKey);

                    return new CommandModel(c.Body.Key, commandFormatter.SerializeCommand(c));
                });
                commandStore.Save(commandModels);

                foreach (Envelope<ICommand> e in commands)
                    commandDispatcher.HandleAsync(e).Wait();
            }
        }
    }
}
