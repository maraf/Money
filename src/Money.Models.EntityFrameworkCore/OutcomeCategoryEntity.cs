using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class OutcomeCategoryEntity
    {
        [Key]
        public Guid OutcomeId { get; set; }
        public virtual OutcomeEntity Outcome { get; set; }

        [Key]
        public Guid CategoryId { get; set; }
        public virtual CategoryEntity Category { get; set; }
    }
}
