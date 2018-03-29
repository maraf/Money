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
    public class CurrencyHandler : AggregateRootCommandHandler<CurrencyList>, 
        ICommandHandler<CreateCurrency>, 
        ICommandHandler<ChangeCurrencySymbol>, 
        ICommandHandler<SetCurrencyAsDefault>, 
        ICommandHandler<SetExchangeRate>, 
        ICommandHandler<RemoveExchangeRate>, 
        ICommandHandler<DeleteCurrency>
    {
        private readonly IKey currencyListKey = GuidKey.Create(
            Guid.Parse("AF215C3D-B228-4004-806B-AC31398660A8"),
            KeyFactory.Empty(typeof(CurrencyList)).Type
        );

        public CurrencyHandler(IFactory<IRepository<CurrencyList, IKey>> repositoryFactory) 
            : base(repositoryFactory)
        { }

        protected override CurrencyList GetAggregate(IRepository<CurrencyList, IKey> repository, IKey key)
        {
            CurrencyList currencies = repository.Find(key);
            if (currencies == null)
                currencies = new CurrencyList();

            return currencies;
        }

        public Task HandleAsync(CreateCurrency command) => WithCommand(command.Key).Execute(currencyListKey, model => model.Add(command.UniqueCode, command.Symbol));
        public Task HandleAsync(ChangeCurrencySymbol command) => WithCommand(command.Key).Execute(currencyListKey, model => model.ChangeSymbol(command.UniqueCode, command.NewSymbol));
        public Task HandleAsync(SetCurrencyAsDefault command) => WithCommand(command.Key).Execute(currencyListKey, model => model.SetAsDefault(command.UniqueCode));
        public Task HandleAsync(SetExchangeRate command) => WithCommand(command.Key).Execute(currencyListKey, model => model.SetExchangeRate(command.SourceUniqueCode, command.TargetUniqueCode, command.ValidFrom, command.Rate));
        public Task HandleAsync(RemoveExchangeRate command) => WithCommand(command.Key).Execute(currencyListKey, model => model.RemoveExchangeRate(command.SourceUniqueCode, command.TargetUniqueCode, command.ValidFrom, command.Rate));
        public Task HandleAsync(DeleteCurrency command) => WithCommand(command.Key).Execute(currencyListKey, model => model.Delete(command.UniqueCode));
    }
}
