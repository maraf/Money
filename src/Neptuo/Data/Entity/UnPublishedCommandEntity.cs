using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data.Entity
{
    public class UnPublishedCommandEntity
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public virtual CommandEntity Command { get; set; }
        public bool IsHandled { get; set; }
        
        public UnPublishedCommandEntity()
        { }

        public UnPublishedCommandEntity(CommandEntity payload)
        {
            Ensure.NotNull(payload, "payload");
            Command = payload;
        }
    }
}
