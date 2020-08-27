using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging.Serialization.Formatters
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
                DateTime.Now.ToString("HH:mm:ss"),
                scopeName,
                level.ToString().ToUpperInvariant(),
                message
            );
        }
    }
}
