using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a user's property value has been changed.
    /// </summary>
    public class UserPropertyChanged : UserEvent
    {
        public string PropertyKey { get; }
        public string Value { get; }

        public UserPropertyChanged(IKey key, IKey aggregateKey, string propertyKey, string value)
            : base(key, aggregateKey, 0)
        {
            Ensure.NotNullOrEmpty(propertyKey, "propertyKey");
            UserKey = aggregateKey;
            PropertyKey = propertyKey;
            Value = value;
        }
    }
}
