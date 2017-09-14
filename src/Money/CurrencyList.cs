using Neptuo.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Events;
using Neptuo.Models.Keys;
using Neptuo;
using Neptuo.Events.Handlers;
using Money.Events;

namespace Money
{
    /// <summary>
    /// All currencies.
    /// </summary>
    public class CurrencyList : AggregateRoot,
        IEventHandler<CurrencyCreated>,
        IEventHandler<CurrencyDefaultChanged>,
        IEventHandler<CurrencyExchangeRateSet>,
        IEventHandler<CurrencySymbolChanged>,
        IEventHandler<CurrencyDeleted>
    {
        private readonly HashSet<string> uniqueCodes = new HashSet<string>();
        private readonly HashSet<string> deletedUniqueCodes = new HashSet<string>();
        private readonly HashSet<int> exchangeRateHashCodes = new HashSet<int>();
        private string defaultName = null;

        public CurrencyList()
        { }

        public CurrencyList(IKey key, IEnumerable<IEvent> events)
            : base(key, events)
        { }

        private void EnsureUnique(string uniqueCode, bool isDeletedIncluded = false)
        {
            uniqueCode = uniqueCode.ToLowerInvariant();

            if (uniqueCodes.Contains(uniqueCode))
                throw new CurrencyAlreadyExistsException();

            if (isDeletedIncluded && deletedUniqueCodes.Contains(uniqueCode))
                throw new CurrencyAlreadyExistsException();
        }

        private void EnsureExists(string uniqueCode, bool isDeletedIncluded = false)
        {
            uniqueCode = uniqueCode.ToLowerInvariant();

            if (uniqueCodes.Contains(uniqueCode))
                return;

            if (isDeletedIncluded && deletedUniqueCodes.Contains(uniqueCode))
                return;

            throw new CurrencyDoesNotExistException();
        }

        public void Add(string uniqueCode, string symbol)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            Ensure.NotNullOrEmpty(symbol, "symbol");
            EnsureUnique(uniqueCode);
            Publish(new CurrencyCreated(uniqueCode, symbol));

            if (defaultName == null)
                Publish(new CurrencyDefaultChanged(uniqueCode));
        }

        Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            return UpdateState(() => uniqueCodes.Add(payload.UniqueCode.ToLowerInvariant()));
        }

        public void SetAsDefault(string uniqueCode)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            EnsureExists(uniqueCode);

            if (defaultName == uniqueCode.ToLowerInvariant())
                throw new CurrencyAlreadyAsDefaultException();

            Publish(new CurrencyDefaultChanged(uniqueCode));
        }

        Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload)
        {
            return UpdateState(() => defaultName = payload.UniqueCode.ToLowerInvariant());
        }

        public void ChangeSymbol(string uniqueCode, string symbol)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            EnsureExists(uniqueCode);

            Publish(new CurrencySymbolChanged(uniqueCode, symbol));
        }

        Task IEventHandler<CurrencySymbolChanged>.HandleAsync(CurrencySymbolChanged payload)
        {
            return Task.CompletedTask;
        }

        public void SetExchangeRate(string sourceUniqueCode, string targetUniqueCode, DateTime validFrom, double rate)
        {
            Ensure.NotNullOrEmpty(sourceUniqueCode, "sourceUniqueCode");
            Ensure.NotNullOrEmpty(targetUniqueCode, "targetUniqueCode");
            EnsureExists(sourceUniqueCode);
            EnsureExists(targetUniqueCode);
            Ensure.Positive(rate, "rate");

            CurrencyExchangeRateSet payload = new CurrencyExchangeRateSet(sourceUniqueCode, targetUniqueCode, validFrom, rate);
            if (exchangeRateHashCodes.Contains(payload.GetHashCode()))
                throw new CurrencyExchangeRateAlreadyExistsException();

            Publish(payload);
        }

        Task IEventHandler<CurrencyExchangeRateSet>.HandleAsync(CurrencyExchangeRateSet payload)
        {
            exchangeRateHashCodes.Add(payload.GetHashCode());
            return Task.CompletedTask;
        }

        public void Delete(string uniqueCode)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            EnsureExists(uniqueCode, true);

            if (defaultName == uniqueCode.ToLowerInvariant())
                throw new CantDeleteDefaultCurrencyException();

            if (uniqueCodes.Count == 1)
                throw new CantDeleteLastCurrencyException();

            Publish(new CurrencyDeleted(uniqueCode));
        }

        Task IEventHandler<CurrencyDeleted>.HandleAsync(CurrencyDeleted payload)
        {
            return UpdateState(() =>
            {
                string uniqueCode = payload.UniqueCode.ToLowerInvariant();
                uniqueCodes.Remove(uniqueCode);
                deletedUniqueCodes.Remove(uniqueCode);
            });
        }
    }
}
