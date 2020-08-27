using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when user password has been changed.
    /// </summary>
    public class PasswordChanged : UserEvent
    {
        public PasswordChanged(IKey key, IKey aggregateKey)
            : base(key, aggregateKey, 0)
        {
            UserKey = aggregateKey;
        }
    }
}
