using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging.Serialization.Formatters
{
    /// <summary>
    /// Default implementation of <see cref="ILogFormatter"/>.
    /// Tries to convert model using <see cref="Converts"/>.
    /// </summary>
    public class DefaultLogFormatter : ILogFormatter
    {
        public string Format(string scopeName, LogLevel level, object model)
        {
            object message;
            if (!Converts.Try(model.GetType(), typeof(string), model, out message))
                message = model;

            return String.Format(
                "{0} {1}({2}){3}{3}{4}{3}",
                DateTime.Now,
                scopeName,
                level.ToString().ToUpperInvariant(),
                Environment.NewLine,
                message
            );
        }
    }
}
