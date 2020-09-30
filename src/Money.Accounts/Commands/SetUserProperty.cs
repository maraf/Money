using Neptuo;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    public class SetUserProperty : Command
    {
        public string PropertyKey { get; }
        public string Value { get; }

        public SetUserProperty(string propertyKey, string value)
        {
            Ensure.NotNullOrEmpty(propertyKey, "propertyKey");
            PropertyKey = propertyKey;
            Value = value;
        }
    }
}
