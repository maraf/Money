using Neptuo;
using Neptuo.Logging;
using Neptuo.Logging.Serialization.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Bootstrap
{
    public class SingleLineLogFormatter : ILogFormatter
    {
        public string Format(string scopeName, LogLevel level, object model)
        {
            object message;
            if (!Converts.Try(model.GetType(), typeof(string), model, out message))
                message = model;

            return String.Format(
                "{0} {1}({2}): {3}",
                DateTime.Now.ToShortTimeString(),
                scopeName,
                level.ToString().ToUpperInvariant(),
                message
            );
        }
    }
}
