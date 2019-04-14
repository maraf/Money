using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when user email has been changed.
    /// </summary>
    public class EmailChanged : UserEvent
    {
        public string Email { get; }

        public EmailChanged(IKey key, IKey aggregateKey, string email)
            : base(key, aggregateKey, 0)
        {
            UserKey = aggregateKey;
            Email = email;
        }
    }
}
