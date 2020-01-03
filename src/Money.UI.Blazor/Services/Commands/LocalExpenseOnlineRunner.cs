using Money.Events;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    public class LocalExpenseOnlineRunner : System.IDisposable
    {
        private readonly NetworkState network;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IEventDispatcher eventDispatcher;
        private readonly CreateExpenseStorage storage;

        public LocalExpenseOnlineRunner(NetworkState network, ICommandDispatcher commandDispatcher, IEventDispatcher eventDispatcher, CreateExpenseStorage storage)
        {
            Ensure.NotNull(network, "network");
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(eventDispatcher, "eventDispatcher");
            Ensure.NotNull(storage, "storage");
            this.network = network;
            this.commandDispatcher = commandDispatcher;
            this.eventDispatcher = eventDispatcher;
            this.storage = storage;
        }

        public void Initialize()
        {
            // TODO: Make automatic...
            //network.StatusChanged += OnNetworkStatusChanged;
            //OnNetworkStatusChanged();
        }

        public void Dispose()
        {
            network.StatusChanged -= OnNetworkStatusChanged;
        }

        private async void OnNetworkStatusChanged()
        {
            await PublishAsync();
        }

        public async Task PublishAsync()
        {
            if (network.IsOnline)
            {
                var commands = await storage.LoadAsync();
                if (commands != null && commands.Count > 0)
                {
                    foreach (CreateOutcome command in commands)
                        await commandDispatcher.HandleAsync(command);

                    await storage.DeleteAsync();
                    await eventDispatcher.PublishAsync(new LocallyStoredExpensesPublished());
                }
            }
        }
    }
}
