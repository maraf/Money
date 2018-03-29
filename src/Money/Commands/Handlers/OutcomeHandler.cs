using Neptuo.Activators;
using Neptuo.Commands.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands.Handlers
{
    public class OutcomeHandler : AggregateRootCommandHandler<Outcome>, 
        ICommandHandler<CreateOutcome>, 
        ICommandHandler<ChangeOutcomeAmount>, 
        ICommandHandler<ChangeOutcomeDescription>, 
        ICommandHandler<ChangeOutcomeWhen>, 
        ICommandHandler<DeleteOutcome>
    {
        public OutcomeHandler(IFactory<IRepository<Outcome, IKey>> repositoryFactory) 
            : base(repositoryFactory)
        { }

        public Task HandleAsync(CreateOutcome command) => WithCommand(command.Key).Execute(() => new Outcome(command.Amount, command.Description, command.When, command.CategoryKey));
        public Task HandleAsync(ChangeOutcomeAmount command) => WithCommand(command.Key).Execute(command.OutcomeKey, model => model.ChangeAmount(command.Amount));
        public Task HandleAsync(ChangeOutcomeDescription command) => WithCommand(command.Key).Execute(command.OutcomeKey, model => model.ChangeDescription(command.Description));
        public Task HandleAsync(ChangeOutcomeWhen command) => WithCommand(command.Key).Execute(command.OutcomeKey, model => model.ChangeWhen(command.When));
        public Task HandleAsync(DeleteOutcome command) => WithCommand(command.Key).Execute(command.OutcomeKey, model => model.Delete());
    }
}
