using Money.Commands;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Commands
{
    public class DeleteOutcomeCommand : Neptuo.Observables.Commands.Command
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IKey outcomeKey;

        private Action executed;

        private bool isExecuted;
        private bool isExecuting;

        public DeleteOutcomeCommand(ICommandDispatcher commandDispatcher, IKey outcomeKey)
        {
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.Condition.NotEmptyKey(outcomeKey);
            this.commandDispatcher = commandDispatcher;
            this.outcomeKey = outcomeKey;
        }

        public DeleteOutcomeCommand AddExecuted(Action executed)
        {
            Ensure.NotNull(executed, "executed");
            this.executed += executed;
            return this;
        }

        public override bool CanExecute()
        {
            return !isExecuted&& !isExecuting;
        }

        public override async void Execute()
        {
            if (CanExecute())
            {
                isExecuting = true;
                RaiseCanExecuteChanged();

                await commandDispatcher.HandleAsync(new DeleteOutcome(outcomeKey));

                isExecuting = false;
                isExecuted = true;
                RaiseCanExecuteChanged();

                if (executed != null)
                {
                    executed();
                    executed = null;
                }
            }
        }
    }
}
