using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data.Entity
{
    public class UnPublishedEventEntity
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public virtual EventEntity Event { get; set; }
        public List<EventPublishedToHandlerEntity> PublishedToHandlers { get; set; }

        public UnPublishedEventEntity()
        { }

        public UnPublishedEventEntity(EventEntity payload)
        {
            Ensure.NotNull(payload, "payload");
            Event = payload;
        }
    }
}
