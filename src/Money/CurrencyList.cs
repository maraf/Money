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
        IEventHandler<CurrencyExchangeRateSet>
    {
        private readonly HashSet<string> names = new HashSet<string>();
        private string defaultName = null;

        public CurrencyList()
        { }

        public CurrencyList(IKey key, IEnumerable<IEvent> events) 
            : base(key, events)
        { }

        public void Add(string name)
        {
            Ensure.NotNullOrEmpty(name, "name");
            if (names.Contains(name.ToLowerInvariant()))
                throw new CurrencyAlreadyExistsException();

            Publish(new CurrencyCreated(name));

            if (defaultName == null)
                Publish(new CurrencyDefaultChanged(name));
        }

        Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            return UpdateState(() => names.Add(payload.Name.ToLowerInvariant()));
        }

        public void SetAsDefault(string name)
        {
            Ensure.NotNullOrEmpty(name, "name");
            if (!names.Contains(name.ToLowerInvariant()))
                throw new CurrencyDoesNotExistException();

            Publish(new CurrencyDefaultChanged(name));
        }

        Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload)
        {
            return UpdateState(() => defaultName = payload.Name.ToLowerInvariant());
        }

        public void SetExchangeRate(string sourceName, string targetName, DateTime validFrom, double rate)
        {
            Ensure.NotNullOrEmpty(sourceName, "sourceName");
            Ensure.NotNullOrEmpty(targetName, "targetName");
            Ensure.Positive(rate, "rate");

            if (!names.Contains(sourceName.ToLowerInvariant()))
                throw new CurrencyDoesNotExistException();

            if (!names.Contains(targetName.ToLowerInvariant()))
                throw new CurrencyDoesNotExistException();

            Publish(new CurrencyExchangeRateSet(sourceName, targetName, validFrom, rate));
        }

        Task IEventHandler<CurrencyExchangeRateSet>.HandleAsync(CurrencyExchangeRateSet payload)
        {
            return UpdateState(() => { });
        }
    }
}
