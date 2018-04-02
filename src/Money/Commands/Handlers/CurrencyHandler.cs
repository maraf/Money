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
        ICommandHandler<Envelope<CreateCurrency>>,
        ICommandHandler<Envelope<ChangeCurrencySymbol>>,
        ICommandHandler<Envelope<SetCurrencyAsDefault>>,
        ICommandHandler<Envelope<SetExchangeRate>>,
        ICommandHandler<Envelope<RemoveExchangeRate>>,
        ICommandHandler<Envelope<DeleteCurrency>>
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

        public Task HandleAsync(Envelope<CreateCurrency> envelope) => WithCommand(envelope.Body.Key).Execute(currencyListKey, envelope, model => model.Add(envelope.Body.UniqueCode, envelope.Body.Symbol));
        public Task HandleAsync(Envelope<ChangeCurrencySymbol> envelope) => WithCommand(envelope.Body.Key).Execute(currencyListKey, envelope, model => model.ChangeSymbol(envelope.Body.UniqueCode, envelope.Body.NewSymbol));
        public Task HandleAsync(Envelope<SetCurrencyAsDefault> envelope) => WithCommand(envelope.Body.Key).Execute(currencyListKey, envelope, model => model.SetAsDefault(envelope.Body.UniqueCode));
        public Task HandleAsync(Envelope<SetExchangeRate> envelope) => WithCommand(envelope.Body.Key).Execute(currencyListKey, envelope, model => model.SetExchangeRate(envelope.Body.SourceUniqueCode, envelope.Body.TargetUniqueCode, envelope.Body.ValidFrom, envelope.Body.Rate));
        public Task HandleAsync(Envelope<RemoveExchangeRate> envelope) => WithCommand(envelope.Body.Key).Execute(currencyListKey, envelope, model => model.RemoveExchangeRate(envelope.Body.SourceUniqueCode, envelope.Body.TargetUniqueCode, envelope.Body.ValidFrom, envelope.Body.Rate));
        public Task HandleAsync(Envelope<DeleteCurrency> envelope) => WithCommand(envelope.Body.Key).Execute(currencyListKey, envelope, model => model.Delete(envelope.Body.UniqueCode));
    }
}
