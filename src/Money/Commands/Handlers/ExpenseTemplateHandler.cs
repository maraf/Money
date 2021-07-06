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
    public class ExpenseTemplateHandler : AggregateRootCommandHandler<ExpenseTemplate>,
        ICommandHandler<Envelope<CreateExpenseTemplate>>,
        ICommandHandler<Envelope<DeleteExpenseTemplate>>
    {
        public ExpenseTemplateHandler(IFactory<IRepository<ExpenseTemplate, IKey>> repositoryFactory)
            : base(repositoryFactory)
        { }

        public Task HandleAsync(Envelope<CreateExpenseTemplate> envelope) => WithCommand(envelope.Body.Key).Execute(envelope, () => new ExpenseTemplate(envelope.Body.Amount, envelope.Body.Description, envelope.Body.CategoryKey));
        public Task HandleAsync(Envelope<DeleteExpenseTemplate> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.ExpenseTemplateKey, envelope, model => model.Delete());
    }
}
