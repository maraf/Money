using Neptuo.Events;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// A base class for all user related events.
    /// </summary>
    public class UserEvent : Event
    {
        /// <summary>
        /// Gets a key of the user.
        /// </summary>
        public IKey UserKey { get; /*internal*/ set; }

        public UserEvent()
        { }

        public UserEvent(IKey key, IKey aggregateKey, int version) 
            : base(key, aggregateKey, version)
        { }
    }
}
