using Money.Services;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Commands
{
    public class DeleteOutcomeCommand : Command
    {
        private readonly IDomainFacade domainFacade;
        private readonly IKey outcomeKey;

        private Action executed;

        private bool isExecuted;
        private bool isExecuting;

        public DeleteOutcomeCommand(IDomainFacade domainFacade, IKey outcomeKey)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.Condition.NotEmptyKey(outcomeKey);
            this.domainFacade = domainFacade;
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

                await domainFacade.DeleteOutcomeAsync(outcomeKey);

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
