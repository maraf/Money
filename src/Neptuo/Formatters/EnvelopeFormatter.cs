using Neptuo.Collections.Specialized;
using Neptuo.Internals;
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
    /// The implementation of <see cref="IFormatter"/> that unwraps any incomming envelope and
    /// pass to the inner formatter the body with the metadata saved in the context.Metadata["EnvelopeMetadata"].
    /// 
    /// When deserializing the object, inner formatter should provide body 
    /// and if context.Metadata["EnvelopeMetadata"] is specified, these are used as envelope metadata.
    /// </summary>
    public class EnvelopeFormatter : IFormatter
    {
        private readonly IFormatter inner;

        /// <summary>
        /// Creates new instance that wraps <paramref name="inner"/>.
        /// </summary>
        /// <param name="inner">The inner formatter for 'real' serialization/deserialization.</param>
        public EnvelopeFormatter(IFormatter inner)
        {
            Ensure.NotNull(inner, "inner");
            this.inner = inner;
        }

        private bool TrySerializeEnvelope(object input, ISerializerContext context, out object body, out DefaultSerializerContext innerContext)
        {
            Envelope envelope = input as Envelope;
            if (envelope != null)
            {
                innerContext = new DefaultSerializerContext(envelope.Body.GetType(), context.Output);
                foreach (string key in context.Metadata.Keys)
                    innerContext.Metadata.Add(key, context.Metadata.Get<object>(key));

                body = envelope.Body;
                innerContext.AddEnvelopeMetadata(envelope.Metadata);
                return true;
            }

            body = null;
            innerContext = null;
            return false;
        }

        public bool TrySerialize(object input, ISerializerContext context)
        {
            object body;
            DefaultSerializerContext innerContext;
            if(TrySerializeEnvelope(input, context, out body, out innerContext))
                return inner.TrySerialize(body, innerContext);

            return inner.TrySerialize(input, context);
        }

        public Task<bool> TrySerializeAsync(object input, ISerializerContext context)
        {
            object body;
            DefaultSerializerContext innerContext;
            if (TrySerializeEnvelope(input, context, out body, out innerContext))
                return inner.TrySerializeAsync(body, innerContext);

            return inner.TrySerializeAsync(input, context);
        }

        private bool TryDeserializeEnvelope(Stream input, IDeserializerContext context, out DefaultDeserializerContext innerContext)
        {
            if (typeof(Envelope).IsAssignableFrom(context.OutputType))
            {
                Type innerType = typeof(object);
                if (context.OutputType.GetTypeInfo().IsGenericType)
                    innerType = context.OutputType.GetGenericArguments()[0];

                innerContext = new DefaultDeserializerContext(innerType);
                foreach (string key in context.Metadata.Keys)
                    innerContext.Metadata.Add(key, context.Metadata.Get<object>(key));

                innerContext.AddEnvelopeMetadata(new KeyValueCollection());
                return true;
            }

            innerContext = null;
            return false;
        }

        private Envelope WrapOutputInEnvelope(Type innerType, object output, IDeserializerContext context)
        {
            Envelope envelope = EnvelopeFactory.Create(output, innerType);

            IKeyValueCollection metadata = context.GetEnvelopeMetadata();
            foreach (string key in metadata.Keys)
                envelope.Metadata.Add(key, metadata.Get<object>(key));

            return envelope;
        }

        public bool TryDeserialize(Stream input, IDeserializerContext context)
        {
            DefaultDeserializerContext innerContext;
            if (TryDeserializeEnvelope(input, context, out innerContext))
            {
                if(inner.TryDeserialize(input, innerContext))
                {
                    context.Output = WrapOutputInEnvelope(innerContext.OutputType, innerContext.Output, innerContext);
                    return true;
                }

                return false;
            }

            return inner.TryDeserialize(input, context);
        }

        public async Task<bool> TryDeserializeAsync(Stream input, IDeserializerContext context)
        {
            DefaultDeserializerContext innerContext;
            if (TryDeserializeEnvelope(input, context, out innerContext))
            {
                if (await inner.TryDeserializeAsync(input, innerContext))
                {
                    context.Output = WrapOutputInEnvelope(innerContext.OutputType, innerContext.Output, innerContext);
                    return true;
                }

                return false;
            }

            return await inner.TryDeserializeAsync(input, context);
        }
    }
}
