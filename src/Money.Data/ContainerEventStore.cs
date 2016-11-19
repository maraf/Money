using Neptuo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Models.Keys;

namespace Money.Data
{
    public class ContainerEventStore : IEventStore
    {
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
