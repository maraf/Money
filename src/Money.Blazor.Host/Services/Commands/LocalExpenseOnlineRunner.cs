using Money.Events;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Logging;
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
        private readonly ILog log;

        public LocalExpenseOnlineRunner(NetworkState network, ICommandDispatcher commandDispatcher, IEventDispatcher eventDispatcher, CreateExpenseStorage storage, ILog<LocalExpenseOnlineRunner> log)
        {
            Ensure.NotNull(network, "network");
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(eventDispatcher, "eventDispatcher");
            Ensure.NotNull(storage, "storage");
            Ensure.NotNull(log, "log");
            this.network = network;
            this.commandDispatcher = commandDispatcher;
            this.eventDispatcher = eventDispatcher;
            this.storage = storage;
            this.log = log;
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
            log.Debug("Publishing..");

            if (network.IsOnline)
            {
                log.Debug("Network is online.");

                var commands = await storage.LoadAsync();
                if (commands != null && commands.Count > 0)
                {
                    log.Debug($"Publishing '{commands.Count}' expenses.");

                    foreach (CreateOutcome command in commands)
                        await commandDispatcher.HandleAsync(command);

                    await storage.DeleteAsync();
                    await eventDispatcher.PublishAsync(new LocallyStoredExpensesPublished());
                }
                else
                {
                    log.Debug($"Local storage is empty.");
                }
            }
        }
    }
}
