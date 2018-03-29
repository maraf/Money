using Neptuo;
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
    public class OutcomeHandler : ICommandHandler<ChangeOutcomeAmount>, ICommandHandler<ChangeOutcomeDescription>, ICommandHandler<ChangeOutcomeWhen>, ICommandHandler<DeleteOutcome>
    {
        private readonly IRepository<Outcome, IKey> repository;

        public OutcomeHandler(IRepository<Outcome, IKey> repository)
        {
            Ensure.NotNull(repository, "repository");
            this.repository = repository;
        }

        public Task HandleAsync(ChangeOutcomeAmount command)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome outcome = repository.Get(command.OutcomeKey);
                outcome.ChangeAmount(command.Amount);
                repository.Save(outcome);
            });
        }

        public Task HandleAsync(ChangeOutcomeDescription command)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome outcome = repository.Get(command.OutcomeKey);
                outcome.ChangeDescription(command.Description);
                repository.Save(outcome);
            });
        }

        public Task HandleAsync(ChangeOutcomeWhen command)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome outcome = repository.Get(command.OutcomeKey);
                outcome.ChangeWhen(command.When);
                repository.Save(outcome);
            });
        }

        public Task HandleAsync(DeleteOutcome command)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome outcome = repository.Get(command.OutcomeKey);
                outcome.Delete();
                repository.Save(outcome);
            });
        }
    }
}
