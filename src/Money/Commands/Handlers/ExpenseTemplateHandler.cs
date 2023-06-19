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
        ICommandHandler<Envelope<ChangeExpenseTemplateAmount>>,
        ICommandHandler<Envelope<ChangeExpenseTemplateDescription>>,
        ICommandHandler<Envelope<ChangeExpenseTemplateWhen>>,
        ICommandHandler<Envelope<ChangeExpenseTemplateFixed>>,
        ICommandHandler<Envelope<DeleteExpenseTemplate>>
    {
        public ExpenseTemplateHandler(IFactory<IRepository<ExpenseTemplate, IKey>> repositoryFactory)
            : base(repositoryFactory)
        { }

        public Task HandleAsync(Envelope<CreateExpenseTemplate> envelope) => WithCommand(envelope.Body.Key).Execute(envelope, () =>
        {
            if (envelope.Body.Version == 1)
                return new ExpenseTemplate(envelope.Body.Amount, envelope.Body.Description, envelope.Body.CategoryKey);
            else
                return new ExpenseTemplate(envelope.Body.Amount, envelope.Body.Description, envelope.Body.CategoryKey, envelope.Body.IsFixed);
        });

        public Task HandleAsync(Envelope<DeleteExpenseTemplate> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.ExpenseTemplateKey, envelope, model => model.Delete());

        public Task HandleAsync(Envelope<ChangeExpenseTemplateAmount> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.ExpenseTemplateKey, envelope, model => model.ChangeAmount(envelope.Body.Amount));

        public Task HandleAsync(Envelope<ChangeExpenseTemplateDescription> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.ExpenseTemplateKey, envelope, model => model.ChangeDescription(envelope.Body.Description));

        public Task HandleAsync(Envelope<ChangeExpenseTemplateWhen> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.ExpenseTemplateKey, envelope, model => model.ChangeWhen(envelope.Body.When));

        public Task HandleAsync(Envelope<ChangeExpenseTemplateFixed> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.ExpenseTemplateKey, envelope, model => model.ChangeFixed(envelope.Body.IsFixed));
    }
}
