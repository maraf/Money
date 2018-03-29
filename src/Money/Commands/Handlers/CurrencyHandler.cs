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
    public class CurrencyHandler : ICommandHandler<CreateCurrency>, ICommandHandler<ChangeCurrencySymbol>, ICommandHandler<SetCurrencyAsDefault>, ICommandHandler<SetExchangeRate>, ICommandHandler<RemoveExchangeRate>, ICommandHandler<DeleteCurrency>
    {
        private readonly IRepository<CurrencyList, IKey> repository;

        private readonly IKey currencyListKey = GuidKey.Create(
            Guid.Parse("AF215C3D-B228-4004-806B-AC31398660A8"),
            KeyFactory.Empty(typeof(CurrencyList)).Type
        );

        public CurrencyHandler(IRepository<CurrencyList, IKey> repository)
        {
            Ensure.NotNull(repository, "repository");
            this.repository = repository;
        }

        public Task HandleAsync(CreateCurrency command)
        {
            return Task.Factory.StartNew(() =>
            {
                CurrencyList currencies = repository.Find(currencyListKey);
                if (currencies == null)
                    currencies = new CurrencyList();

                currencies.Add(command.UniqueCode, command.Symbol);
                repository.Save(currencies);
            });
        }

        public Task HandleAsync(ChangeCurrencySymbol command)
        {
            return Task.Factory.StartNew(() =>
            {
                CurrencyList currencies = repository.Find(currencyListKey);
                if (currencies == null)
                    currencies = new CurrencyList();

                currencies.ChangeSymbol(command.UniqueCode, command.NewSymbol);
                repository.Save(currencies);
            });
        }

        public Task HandleAsync(SetCurrencyAsDefault command)
        {
            return Task.Factory.StartNew(() =>
            {
                CurrencyList currencies = repository.Get(currencyListKey);
                currencies.SetAsDefault(command.UniqueCode);
                repository.Save(currencies);
            });
        }

        public Task HandleAsync(SetExchangeRate command)
        {
            return Task.Factory.StartNew(() =>
            {
                CurrencyList currencies = repository.Get(currencyListKey);
                currencies.SetExchangeRate(command.SourceUniqueCode, command.TargetUniqueCode, command.ValidFrom, command.Rate);
                repository.Save(currencies);
            });
        }

        public Task HandleAsync(RemoveExchangeRate command)
        {
            return Task.Factory.StartNew(() =>
            {
                CurrencyList currencies = repository.Get(currencyListKey);
                currencies.RemoveExchangeRate(command.SourceUniqueCode, command.TargetUniqueCode, command.ValidFrom, command.Rate);
                repository.Save(currencies);
            });
        }

        public Task HandleAsync(DeleteCurrency command)
        {
            return Task.Factory.StartNew(() =>
            {
                CurrencyList currencies = repository.Get(currencyListKey);
                currencies.Delete(command.UniqueCode);
                repository.Save(currencies);
            });
        }
    }
}
