using Neptuo;
using Neptuo.Activators;
using Neptuo.Commands;
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
        ICommandHandler<Envelope<ChangeExpenseTemplateCategory>>,
        ICommandHandler<Envelope<ChangeExpenseTemplateFixed>>,
        ICommandHandler<Envelope<SetExpenseTemplateMonthlyRecurrence>>,
        ICommandHandler<Envelope<SetExpenseTemplateXMonthsRecurrence>>,
        ICommandHandler<Envelope<SetExpenseTemplateYearlyRecurrence>>,
        ICommandHandler<Envelope<SetExpenseTemplateSingleRecurrence>>,
        ICommandHandler<Envelope<ClearExpenseTemplateRecurrence>>,
        ICommandHandler<Envelope<DeleteExpenseTemplate>>
    {
        public ExpenseTemplateHandler(IFactory<IRepository<ExpenseTemplate, IKey>> repositoryFactory)
            : base(repositoryFactory)
        { }

        private Task HandleAsync<T>(Envelope<T> envelope, Action<ExpenseTemplate, T> handler) where T: Command, IExpenseTemplateCommand
            => WithCommand(envelope.Body.Key).Execute(envelope.Body.ExpenseTemplateKey, envelope, model => handler(model, envelope.Body));

        public Task HandleAsync(Envelope<CreateExpenseTemplate> envelope) => WithCommand(envelope.Body.Key).Execute(envelope, () =>
        {
            if (envelope.Body.Version == 1)
                return new ExpenseTemplate(envelope.Body.Amount, envelope.Body.Description, envelope.Body.CategoryKey);
            else
                return new ExpenseTemplate(envelope.Body.Amount, envelope.Body.Description, envelope.Body.CategoryKey, envelope.Body.IsFixed);
        });

        public Task HandleAsync(Envelope<DeleteExpenseTemplate> envelope) => HandleAsync(envelope, (model, command) => model.Delete());
        public Task HandleAsync(Envelope<ChangeExpenseTemplateAmount> envelope) => HandleAsync(envelope, (model, command) => model.ChangeAmount(command.Amount));
        public Task HandleAsync(Envelope<ChangeExpenseTemplateDescription> envelope) => HandleAsync(envelope, (model, command) => model.ChangeDescription(command.Description));
        public Task HandleAsync(Envelope<ChangeExpenseTemplateCategory> envelope) => HandleAsync(envelope, (model, command) => model.ChangeCategory(command.CategoryKey));
        public Task HandleAsync(Envelope<ChangeExpenseTemplateFixed> envelope) => HandleAsync(envelope, (model, command) => model.ChangeFixed(command.IsFixed));
        public Task HandleAsync(Envelope<SetExpenseTemplateMonthlyRecurrence> envelope) => HandleAsync(envelope, (model, command) => model.SetMonthlyRecurrence(command.DayInPeriod));
        public Task HandleAsync(Envelope<SetExpenseTemplateXMonthsRecurrence> envelope) => HandleAsync(envelope, (model, command) => model.SetXMonthsRecurrence(command.EveryXPeriods, command.DayInPeriod));
        public Task HandleAsync(Envelope<SetExpenseTemplateYearlyRecurrence> envelope) => HandleAsync(envelope, (model, command) => model.SetYearlyRecurrence(command.MonthInPeriod, command.DayInPeriod));
        public Task HandleAsync(Envelope<SetExpenseTemplateSingleRecurrence> envelope) => HandleAsync(envelope, (model, command) => model.SetSingleRecurrence(command.DueDate));
        public Task HandleAsync(Envelope<ClearExpenseTemplateRecurrence> envelope) => HandleAsync(envelope, (model, command) => model.ClearRecurrence());
    }
}
