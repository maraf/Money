using Neptuo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Models.Keys;
using Neptuo;
using Windows.Storage;

namespace Money.Data
{
    public class ContainerEventStore : IEventStore
    {
        private readonly ApplicationDataContainer container;

        public ContainerEventStore(ApplicationDataContainer container)
        {
            Ensure.NotNull(container, "container");
            this.container = container;
        }

        public IEnumerable<EventModel> Get(IKey aggregateKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EventModel> Get(IKey aggregateKey, int version)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<EventModel> events)
        {
            throw new NotImplementedException();
        }
    }
}
