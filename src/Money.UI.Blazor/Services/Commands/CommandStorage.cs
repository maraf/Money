using Money.Events;
using Neptuo;
using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    public class CommandStorage
    {
        private readonly CreateExpenseStorage expenseStorage;
        private readonly IEventDispatcher eventDispatcher;

        public CommandStorage(CreateExpenseStorage expenseStorage, IEventDispatcher eventDispatcher)
        {
            Ensure.NotNull(expenseStorage, "expenseStorage");
            Ensure.NotNull(eventDispatcher, "eventDispatcher");
            this.expenseStorage = expenseStorage;
            this.eventDispatcher = eventDispatcher;
        }

        public async Task StoreAsync<TCommand>(TCommand command)
        {
            if (command is CreateOutcome expense)
            {
                var expenses = await expenseStorage.LoadAsync() ?? new List<CreateOutcome>();
                expenses.Add(expense);
                await expenseStorage.SaveAsync(expenses);
                await eventDispatcher.PublishAsync(new LocallyStoredExpenseCreated());
            }
            else
            {
                throw Ensure.Exception.InvalidOperation($"Command '{command.GetType().Name}' is not support offline yet.");
            }
        }
    }
}
