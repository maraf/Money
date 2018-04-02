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
        /// <param name="extenders">A list of additional payload extenders.</param>
        public CompositeCommandFormatter(ICompositeTypeProvider provider, IFactory<ICompositeStorage> storageFactory, List<ICompositeFormatterExtender> extenders = null)
        {
            if (extenders == null)
                extenders = new List<ICompositeFormatterExtender>();

            extenders.Add(new CommandExtender());
            envelopeFormatter = new EnvelopeFormatter(new EnvelopeMetadataProcessingFormatter(provider, storageFactory, extenders));
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
    }
}
