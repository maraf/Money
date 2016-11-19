using Neptuo.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// The extensions used in the <see cref="contextFormatter"/> for the <see cref="ISerializerContext"/>.
    /// </summary>
    public static class _FormatterContextExtensions
    {
        #region EnvelopeMetadata

        /// <summary>
        /// Sets 'Metadata' to the metadata of the <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context which metadata to extend.</param>
        /// <param name="value">The vale of the 'Metadata'.</param>
        /// <returns><paramref name="envolope"/>(for fluency).</returns>
        public static ISerializerContext AddEnvelopeMetadata(this DefaultSerializerContext context, IReadOnlyKeyValueCollection value)
        {
            Ensure.NotNull(context, "context");
            context.Metadata.Add("EnvelopeMetadata", value);
            return context;
        }

        /// <summary>
        /// Tries to read 'Metadata' from the metadata of the <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context to read the metadata from.</param>
        /// <param name="value">The value of the 'Metadata'.</param>
        /// <returns><c>true</c> if 'Metadata' can is contained in the <paramref name="context"/>; <c>false</c> otherwise.</returns>
        public static bool TryGetEnvelopeMetadata(this ISerializerContext context, out IReadOnlyKeyValueCollection value)
        {
            Ensure.NotNull(context, "context");
            return context.Metadata.TryGet("EnvelopeMetadata", out value);
        }

        /// <summary>
        /// Reads 'Metadata' from the metadata of the <paramref name="context"/> or throws <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="context">The context to read the metadata from.</param>
        /// <returns>The value associated with the 'Metadata' or throws <see cref="InvalidOperationException"/>.</returns>
        /// <exception cref="InvalidOperationException">When the 'Metadata' is not contained in the metadata of the <paramref name="context"/>.</exception>
        public static IReadOnlyKeyValueCollection GetEnvelopeMetadata(this ISerializerContext context)
        {
            Ensure.NotNull(context, "context");
            return context.Metadata.Get<IReadOnlyKeyValueCollection>("EnvelopeMetadata");
        }


        /// <summary>
        /// Sets 'Metadata' to the metadata of the <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context which metadata to extend.</param>
        /// <param name="value">The vale of the 'Metadata'.</param>
        /// <returns><paramref name="envolope"/>(for fluency).</returns>
        public static IDeserializerContext AddEnvelopeMetadata(this DefaultDeserializerContext context, IKeyValueCollection value)
        {
            Ensure.NotNull(context, "context");
            context.Metadata.Add("EnvelopeMetadata", value);
            return context;
        }

        /// <summary>
        /// Tries to read 'Metadata' from the metadata of the <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context to read the metadata from.</param>
        /// <param name="value">The value of the 'Metadata'.</param>
        /// <returns><c>true</c> if 'Metadata' can is contained in the <paramref name="context"/>; <c>false</c> otherwise.</returns>
        public static bool TryGetEnvelopeMetadata(this IDeserializerContext context, out IKeyValueCollection value)
        {
            Ensure.NotNull(context, "context");
            return context.Metadata.TryGet("EnvelopeMetadata", out value);
        }

        /// <summary>
        /// Reads 'Metadata' from the metadata of the <paramref name="context"/> or throws <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="context">The context to read the metadata from.</param>
        /// <returns>The value associated with the 'Metadata' or throws <see cref="InvalidOperationException"/>.</returns>
        /// <exception cref="InvalidOperationException">When the 'Metadata' is not contained in the metadata of the <paramref name="context"/>.</exception>
        public static IKeyValueCollection GetEnvelopeMetadata(this IDeserializerContext context)
        {
            Ensure.NotNull(context, "context");
            return context.Metadata.Get<IKeyValueCollection>("EnvelopeMetadata");
        }

        #endregion

    }
}
