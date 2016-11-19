using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data
{
    public class EventPublishingModel
    {
        public EventModel Event { get; private set; }
        public IEnumerable<string> PublishedHandlerIdentifiers { get; private set; }

        public EventPublishingModel(EventModel eventModel, IEnumerable<string> publishedHandlerIdentifiers)
        {
            Ensure.NotNull(eventModel, "eventModel");
            Ensure.NotNull(publishedHandlerIdentifiers, "publishedHandlerIdentifiers");
            Event = eventModel;
            PublishedHandlerIdentifiers = publishedHandlerIdentifiers;
        }
    }
}
