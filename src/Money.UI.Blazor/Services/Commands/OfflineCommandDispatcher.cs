using Money.Services;
using Neptuo;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    internal class OfflineCommandDispatcher : ICommandDispatcher
    {
        private readonly HttpCommandDispatcher inner;
        private readonly NetworkState network;
        private readonly CommandStorage storage;

        public OfflineCommandDispatcher(HttpCommandDispatcher inner, NetworkState network, CommandStorage storage)
        {
            Ensure.NotNull(inner, "inner");
            Ensure.NotNull(network, "network");
            Ensure.NotNull(storage, "expenseStorage");
            this.inner = inner;
            this.network = network;
            this.storage = storage;
        }

        public Task HandleAsync<TCommand>(TCommand command)
        {
            if (network.IsOnline)
                return inner.HandleAsync(command);
            else
                return storage.StoreAsync(command);
        }
    }
}
