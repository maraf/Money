using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data.Entity
{
    public class EventEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public Guid EventID { get; set; }
        public string EventType { get; set; }

        public Guid AggregateID { get; set; }
        public string AggregateType { get; set; }
        public string Payload { get; set; }
        public int Version { get; set; }

        public EventModel ToModel()
        {
            return new EventModel(
                GuidKey.Create(AggregateID, AggregateType), 
                GuidKey.Create(EventID, EventType), 
                Payload, 
                Version
            );
        }

        public static EventEntity FromModel(EventModel model)
        {
            Ensure.NotNull(model, "model");

            GuidKey aggregateKey = model.AggregateKey as GuidKey;
            if (aggregateKey == null)
                throw Ensure.Exception.NotGuidKey(model.AggregateKey.GetType(), "aggregateKey");

            GuidKey eventKey = model.EventKey as GuidKey;
            if(eventKey == null)
                throw Ensure.Exception.NotGuidKey(model.EventKey.GetType(), "eventKey");
            
            return new EventEntity()
            {
                EventID = eventKey.Guid,
                EventType = eventKey.Type,

                AggregateID = aggregateKey.Guid,
                AggregateType = aggregateKey.Type,

                Payload = model.Payload,
                Version = model.Version
            };
        }
    }
}
