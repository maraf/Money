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
        private readonly ServerConnectionState serverConnection;
        private readonly CommandStorage storage;

        public OfflineCommandDispatcher(HttpCommandDispatcher inner, ServerConnectionState serverConnection, CommandStorage storage)
        {
            Ensure.NotNull(inner, "inner");
            Ensure.NotNull(serverConnection, "serverConnection");
            Ensure.NotNull(storage, "expenseStorage");
            this.inner = inner;
            this.serverConnection = serverConnection;
            this.storage = storage;
        }

        public Task HandleAsync<TCommand>(TCommand command)
        {
            if (serverConnection.IsAvailable)
                return inner.HandleAsync(command);
            else
                return storage.StoreAsync(command);
        }
    }
}
