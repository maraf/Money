using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Data
{
    internal class EventSourcingContext : Neptuo.Data.Entity.EventSourcingContext
    {
        public EventSourcingContext() 
            : base("Filename=EventSourcing.db")
        {
        }
    }
}
