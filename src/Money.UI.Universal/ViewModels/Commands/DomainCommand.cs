using Neptuo;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Commands
{
    public abstract class DomainCommand<T> : Neptuo.Observables.Commands.Command
        where T : Command
    {
        private readonly ICommandDispatcher commandDispatcher;

        public DomainCommand(ICommandDispatcher commandDispatcher)
        {
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            this.commandDispatcher = commandDispatcher;
        }

        public override void Execute()
        {
            commandDispatcher.HandleAsync(CreateDomainCommand());
        }

        protected abstract T CreateDomainCommand();
    }
}
