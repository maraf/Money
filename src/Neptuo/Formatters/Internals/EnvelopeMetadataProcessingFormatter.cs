using Neptuo;
using Neptuo.Activators;
using Neptuo.Collections.Specialized;
using Neptuo.Commands;
using Neptuo.Formatters.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Internals
{
    internal class EnvelopeMetadataProcessingFormatter : CompositeTypeFormatter
    {
        public const string MetadataKey = "Metadata";

        private readonly IEnumerable<ICompositeFormatterExtender> extenders;

        public EnvelopeMetadataProcessingFormatter(ICompositeTypeProvider provider, IFactory<ICompositeStorage> storageFactory, params ICompositeFormatterExtender[] extenders)
            : this(provider, storageFactory, (IEnumerable<ICompositeFormatterExtender>)extenders)
        { }

        public EnvelopeMetadataProcessingFormatter(ICompositeTypeProvider provider, IFactory<ICompositeStorage> storageFactory, IEnumerable<ICompositeFormatterExtender> extenders)
            : base(provider, storageFactory)
        {
            Ensure.NotNull(extenders, "extenders");
            this.extenders = extenders;
        }

        protected override bool TryLoad(Stream input, IDeserializerContext context, CompositeType type, ICompositeStorage storage)
        {
            if (base.TryLoad(input, context, type, storage))
            {
                ICompositeStorage payloadStorage = GetOrAddPayloadStorage(storage);
                foreach (ICompositeFormatterExtender extender in extenders)
                    extender.Load(payloadStorage, context.Output);

                IKeyValueCollection metadata;
                ICompositeStorage metadataStorage;
                if (context.TryGetEnvelopeMetadata(out metadata) && storage.TryGet(MetadataKey, out metadataStorage))
                {
                    foreach (string key in metadataStorage.Keys)
                        metadata.Add(key, metadataStorage.Get<object>(key));
                }

                return true;
            }

            return false;
        }

        protected override bool TryStore(object input, ISerializerContext context, CompositeType type, CompositeVersion typeVersion, ICompositeStorage storage)
        {
            if (base.TryStore(input, context, type, typeVersion, storage))
            {
                ICompositeStorage payloadStorage = GetOrAddPayloadStorage(storage);
                foreach (ICompositeFormatterExtender extender in extenders)
                    extender.Store(payloadStorage, input);

                IReadOnlyKeyValueCollection metadata;
                if (context.TryGetEnvelopeMetadata(out metadata))
                {
                    ICompositeStorage metadataStorage = storage.Add(MetadataKey);
                    foreach (string key in metadata.Keys)
                        metadataStorage.Add(key, metadata.Get<object>(key));
                }

                return true;
            }

            return false;
        }

        private ICompositeStorage GetOrAddPayloadStorage(ICompositeStorage storage)
        {
            ICompositeStorage payloadStorage;
            if (storage.TryGet(Name.Payload, out payloadStorage))
                return payloadStorage;

            return storage.Add(Name.Payload);
        }
    }
}
