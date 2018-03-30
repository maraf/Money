using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data.Entity
{
    public class EventPublishedToHandlerEntity
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public virtual UnPublishedEventEntity Event { get; set; }
        public string HandlerIdentifier { get; set; }

        public EventPublishedToHandlerEntity()
        { }

        public EventPublishedToHandlerEntity(string handlerIdentifier)
        {
            Ensure.NotNullOrEmpty(handlerIdentifier, "handlerIdentifier");
            HandlerIdentifier = handlerIdentifier;
        }
    }
}
