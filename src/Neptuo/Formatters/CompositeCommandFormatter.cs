using Neptuo.Activators;
using Neptuo.Collections.Specialized;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Formatters.Internals;
using Neptuo.Formatters.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// The complete, 'ready to use' formatter for commands with support for envelopes and internal command properties.
    /// </summary>
    public class CompositeCommandFormatter : IFormatter
    {
        private readonly EnvelopeFormatter envelopeFormatter;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="provider">The provider for reading composite type definitions.</param>
        /// <param name="storageFactory">The factory for storage.</param>
        public CompositeCommandFormatter(ICompositeTypeProvider provider, IFactory<ICompositeStorage> storageFactory)
        {
            envelopeFormatter = new EnvelopeFormatter(new Formatter(provider, storageFactory));
        }

        public bool TrySerialize(object input, ISerializerContext context)
        {
            return envelopeFormatter.TrySerialize(input, context);
        }

        public Task<bool> TrySerializeAsync(object input, ISerializerContext context)
        {
            return envelopeFormatter.TrySerializeAsync(input, context);
        }

        public bool TryDeserialize(Stream input, IDeserializerContext context)
        {
            return envelopeFormatter.TryDeserialize(input, context);
        }

        public Task<bool> TryDeserializeAsync(Stream input, IDeserializerContext context)
        {
            return envelopeFormatter.TryDeserializeAsync(input, context);
        }

        /// <summary>
        /// The inner formatter used inside of the <see cref="EnvelopeFormatter"/>.
        /// </summary>
        private class Formatter : EnvelopeMetadataProcessingFormatter
        {
            private readonly CommandExtender commands = new CommandExtender();

            public Formatter(ICompositeTypeProvider provider, IFactory<ICompositeStorage> storageFactory)
                : base(provider, storageFactory)
            { }

            protected override void TryLoad(ICompositeStorage payloadStorage, object output)
            {
                Command command = output as Command;
                if (command != null)
                    commands.Load(payloadStorage, command);
            }

            protected override void TryStore(ICompositeStorage payloadStorage, object input)
            {
                Command command = input as Command;
                if (command != null)
                    commands.Store(payloadStorage, command);
            }
        }
    }
}
