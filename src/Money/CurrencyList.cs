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
        IEventHandler<CurrencyCreated>
    {
        private readonly HashSet<string> names = new HashSet<string>();

        public CurrencyList()
        { }

        internal CurrencyList(IKey key, IEnumerable<IEvent> events) 
            : base(key, events)
        { }

        public void Add(string name)
        {
            Ensure.NotNullOrEmpty(name, "name");
            if (names.Contains(name.ToLowerInvariant()))
                throw new CurrencyAlreadyExistsException();

            Publish(new CurrencyCreated(name));
        }

        Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            return UpdateState(() => names.Add(payload.Name.ToLowerInvariant()));
        }
    }
}
