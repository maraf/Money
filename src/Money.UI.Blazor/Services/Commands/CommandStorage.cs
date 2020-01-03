using Neptuo;
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

        public CommandStorage(CreateExpenseStorage expenseStorage)
        {
            Ensure.NotNull(expenseStorage, "expenseStorage");
            this.expenseStorage = expenseStorage;
        }

        public async Task StoreAsync<TCommand>(TCommand command)
        {
            if (command is CreateOutcome expense)
            {
                var expenses = await expenseStorage.LoadAsync() ?? new List<CreateOutcome>();
                expenses.Add(expense);
                await expenseStorage.SaveAsync(expenses);
            }
            else
            {
                throw Ensure.Exception.InvalidOperation($"Command '{command.GetType().Name}' is not support offline yet.");
            }
        }
    }
}
