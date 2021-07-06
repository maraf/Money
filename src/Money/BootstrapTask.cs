using Money.Commands.Handlers;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Bootstrap;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    public class BootstrapTask : IBootstrapTask
    {
        private readonly ICommandHandlerCollection commandHandlers;
        private readonly IFactory<IRepository<Outcome, IKey>> outcomeRepository;
        private readonly IFactory<IRepository<ExpenseTemplate, IKey>> expenseTemplateRepository;
        private readonly IFactory<IRepository<Income, IKey>> incomeRepository;
        private readonly IFactory<IRepository<Category, IKey>> categoryRepository;
        private readonly IFactory<IRepository<CurrencyList, IKey>> currencyListRepository;

        public BootstrapTask(ICommandHandlerCollection commandHandlers, 
            IFactory<IRepository<Outcome, IKey>> outcomeRepository,
            IFactory<IRepository<ExpenseTemplate, IKey>> expenseTemplateRepository,
            IFactory<IRepository<Income, IKey>> incomeRepository,
            IFactory<IRepository<Category, IKey>> categoryRepository,
            IFactory<IRepository<CurrencyList, IKey>> currencyListRepository)
        {
            Ensure.NotNull(commandHandlers, "commandHandlers");
            Ensure.NotNull(outcomeRepository, "outcomeRepository");
            Ensure.NotNull(expenseTemplateRepository, "expenseTemplateRepository");
            Ensure.NotNull(incomeRepository, "incomeRepository");
            Ensure.NotNull(categoryRepository, "categoryRepository");
            Ensure.NotNull(currencyListRepository, "currencyListRepository");
            this.commandHandlers = commandHandlers;
            this.outcomeRepository = outcomeRepository;
            this.expenseTemplateRepository = expenseTemplateRepository;
            this.incomeRepository = incomeRepository;
            this.categoryRepository = categoryRepository;
            this.currencyListRepository = currencyListRepository;
        }

        public void Initialize()
        {
            OutcomeHandler outcomeHandler = new OutcomeHandler(outcomeRepository);
            commandHandlers.AddAll(outcomeHandler);

            ExpenseTemplateHandler expenseTemplateHandler = new ExpenseTemplateHandler(expenseTemplateRepository);
            commandHandlers.AddAll(expenseTemplateHandler);

            IncomeHandler incomeHandler = new IncomeHandler(incomeRepository);
            commandHandlers.AddAll(incomeHandler);

            CategoryHandler categoryHandler = new CategoryHandler(categoryRepository);
            commandHandlers.AddAll(categoryHandler);

            CurrencyHandler currencyHandler = new CurrencyHandler(currencyListRepository);
            commandHandlers.AddAll(currencyHandler);
        }
    }
}
