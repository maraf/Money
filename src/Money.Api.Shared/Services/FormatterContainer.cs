using Neptuo;
using Neptuo.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class FormatterContainer
    {
        public IFormatter Command { get; private set; }
        public IFormatter Event { get; private set; }
        public IFormatter Query { get; private set; }
        public IFormatter Exception { get; private set; }

        public FormatterContainer(IFormatter command, IFormatter eventFormatter, IFormatter query, IFormatter exception)
        {
            Ensure.NotNull(command, "command");
            Ensure.NotNull(eventFormatter, "eventFormatter");
            Ensure.NotNull(query, "query");
            Ensure.NotNull(exception, "exception");
            Command = command;
            Event = eventFormatter;
            Query = query;
            Exception = exception;
        }
    }
}
