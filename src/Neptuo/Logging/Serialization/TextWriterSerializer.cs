using Neptuo.Logging.Serialization.Filters;
using Neptuo.Logging.Serialization.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging.Serialization
{
    /// <summary>
    /// Base implementation of <see cref="ILogSerializer"/> that writes to the <see cref="TextWriter"/>.
    /// </summary>
    public class TextWriterSerializer : TextLogSerializerBase
    {
        /// <summary>
        /// Serializer output writer.
        /// </summary>
        public TextWriter Output { get; private set; }

        /// <summary>
        /// Creates new instance that writes to the <paramref name="output"/>.
        /// </summary>
        /// <param name="output">Serializer output writer.</param>
        public TextWriterSerializer(TextWriter output)
            : this(output, new DefaultLogFormatter(), new AllowedLogFilter())
        { }

        /// <summary>
        /// Creates new instance that writes to the <paramref name="output"/>.
        /// </summary>
        /// <param name="output">Serializer output writer.</param>
        /// <param name="formatter">Log entry formatter.</param>
        /// <param name="filter">Log entry filter.</param>
        public TextWriterSerializer(TextWriter output, ILogFormatter formatter, ILogFilter filter)
            : base(formatter, filter)
        {
            Ensure.NotNull(output, "output");
            Output = output;
        }

        protected override void Append(string scopeName, LogLevel level, string message)
        {
            Output.WriteLine(message);
        }
    }
}
