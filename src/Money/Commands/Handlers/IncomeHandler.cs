using Neptuo;
using Neptuo.Activators;
using Neptuo.Commands.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands.Handlers
{
    public class IncomeHandler : AggregateRootCommandHandler<Income>,
        ICommandHandler<Envelope<CreateIncome>>,
        ICommandHandler<Envelope<ChangeIncomeAmount>>,
        ICommandHandler<Envelope<ChangeIncomeDescription>>,
        ICommandHandler<Envelope<ChangeIncomeWhen>>,
        ICommandHandler<Envelope<DeleteIncome>>
    {
        public IncomeHandler(IFactory<IRepository<Income, IKey>> repositoryFactory)
            : base(repositoryFactory)
        { }

        public Task HandleAsync(Envelope<CreateIncome> envelope) => WithCommand(envelope.Body.Key).Execute(envelope, () => new Income(envelope.Body.Amount, envelope.Body.Description, envelope.Body.When));
        public Task HandleAsync(Envelope<ChangeIncomeAmount> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.IncomeKey, envelope, model => model.ChangeAmount(envelope.Body.Amount));
        public Task HandleAsync(Envelope<ChangeIncomeDescription> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.IncomeKey, envelope, model => model.ChangeDescription(envelope.Body.Description));
        public Task HandleAsync(Envelope<ChangeIncomeWhen> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.IncomeKey, envelope, model => model.ChangeWhen(envelope.Body.When));
        public Task HandleAsync(Envelope<DeleteIncome> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.IncomeKey, envelope, model => model.Delete());
    }
}
