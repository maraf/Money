using Neptuo;
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
        ICommandHandler<Envelope<CreateOutcome>>,
        ICommandHandler<Envelope<ChangeOutcomeAmount>>,
        ICommandHandler<Envelope<ChangeOutcomeDescription>>,
        ICommandHandler<Envelope<ChangeOutcomeWhen>>,
        ICommandHandler<Envelope<DeleteOutcome>>
    {
        public OutcomeHandler(IFactory<IRepository<Outcome, IKey>> repositoryFactory)
            : base(repositoryFactory)
        { }

        public Task HandleAsync(Envelope<CreateOutcome> envelope) => WithCommand(envelope.Body.Key).Execute(envelope, () =>
        {
            if (envelope.Body.Version == 1)
                return new Outcome(envelope.Body.Amount, envelope.Body.Description, envelope.Body.When, envelope.Body.CategoryKey);
            else if (envelope.Body.Version == 2)
                return new Outcome(envelope.Body.Amount, envelope.Body.Description, envelope.Body.When, envelope.Body.CategoryKey, envelope.Body.IsFixed);
            else
                throw Ensure.Exception.InvalidOperation($"Not support version '{envelope.Body.Version}' of '{nameof(CreateOutcome)}' command.");
        });
        public Task HandleAsync(Envelope<ChangeOutcomeAmount> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.OutcomeKey, envelope, model => model.ChangeAmount(envelope.Body.Amount));
        public Task HandleAsync(Envelope<ChangeOutcomeDescription> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.OutcomeKey, envelope, model => model.ChangeDescription(envelope.Body.Description));
        public Task HandleAsync(Envelope<ChangeOutcomeWhen> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.OutcomeKey, envelope, model => model.ChangeWhen(envelope.Body.When));
        public Task HandleAsync(Envelope<DeleteOutcome> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.OutcomeKey, envelope, model => model.Delete());
    }
}
