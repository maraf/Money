using Neptuo.Collections.Specialized;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Internals
{
    /// <summary>
    /// The common extensions for the <see cref="Envelope"/>
    /// </summary>
    internal static class _EnvelopeExtensions
    {
        #region Delay

        /// <summary>
        /// Sets 'Delay' to the metadata of the <paramref name="envelope"/>.
        /// </summary>
        /// <typeparam name="T">The type of the envelope body.</typeparam>
        /// <param name="envelope">The envelope which metadata to extend.</param>
        /// <param name="value">The vale of the 'Delay'.</param>
        /// <returns><paramref name="envolope"/>(for fluency).</returns>
        public static Envelope AddDelay(this Envelope envelope, TimeSpan value)
        {
            Ensure.NotNull(envelope, "envelope");
            envelope.Metadata.Add("Delay", value);
            return envelope;
        }

        /// <summary>
        /// Tries to read 'Delay' from the metadata of the <paramref name="envelope"/>.
        /// </summary>
        /// <typeparam name="T">The type of the envelope body.</typeparam>
        /// <param name="envelope">The envelope to read the metadata from.</param>
        /// <param name="value">The value of the 'Delay'.</param>
        /// <returns><c>true</c> if 'Delay' can is contained in the <paramref name="envelope"/>; <c>false</c> otherwise.</returns>
        public static bool TryGetDelay(this Envelope envelope, out TimeSpan value)
        {
            Ensure.NotNull(envelope, "envelope");
            return envelope.Metadata.TryGet("Delay", out value);
        }

        /// <summary>
        /// Reads 'Delay' from the metadata of the <paramref name="envelope"/> or throws <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <typeparam name="T">The type of the envelope body.</typeparam>
        /// <param name="envelope">The envelope to read the metadata from.</param>
        /// <returns>The value associated with the 'Delay' or throws <see cref="InvalidOperationException"/>.</returns>
        /// <exception cref="InvalidOperationException">When the 'Delay' is not contained in the metadata of the <paramref name="envelope"/>.</exception>
        public static TimeSpan GetDelay(this Envelope envelope)
        {
            Ensure.NotNull(envelope, "envelope");
            return envelope.Metadata.Get<TimeSpan>("Delay");
        }

        #endregion

        #region SourceCommandKey

        /// <summary>
        /// Sets 'SourceCommandKey' to the metadata of the <paramref name="envelope"/>.
        /// </summary>
        /// <param name="envelope">The envelope which metadata to extend.</param>
        /// <param name="value">The vale of the 'SourceCommandKey'.</param>
        /// <returns><paramref name="envelope"/>(for fluency).</returns>
        public static Envelope AddSourceCommandKey(this Envelope envelope, IKey value)
        {
            Ensure.NotNull(envelope, "envelope");
            envelope.Metadata.Add("SourceCommandKey", value);
            return envelope;
        }

        /// <summary>
        /// Tries to read 'SourceCommandKey' from the metadata of the <paramref name="envelope"/>.
        /// </summary>
        /// <param name="envelope">The envelope to read the metadata from.</param>
        /// <param name="value">The value of the 'SourceCommandKey'.</param>
        /// <returns><c>true</c> if 'SourceCommandKey' can is contained in the <paramref name="envelope"/>; <c>false</c> otherwise.</returns>
        public static bool TryGetSourceCommandKey(this Envelope envelope, out IKey value)
        {
            Ensure.NotNull(envelope, "envelope");
            return envelope.Metadata.TryGet("SourceCommandKey", out value);
        }

        /// <summary>
        /// Reads 'SourceCommandKey' from the metadata of the <paramref name="envelope"/> or throws <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="envelope">The envelope to read the metadata from.</param>
        /// <returns>The value associated with the 'SourceCommandKey' or throws <see cref="InvalidOperationException"/>.</returns>
        /// <exception cref="InvalidOperationException">When the 'SourceCommandKey' is not contained in the metadata of the <paramref name="envelope"/>.</exception>
        public static IKey GetSourceCommandKey(this Envelope envelope)
        {
            Ensure.NotNull(envelope, "envelope");
            return envelope.Metadata.Get<IKey>("SourceCommandKey");
        }

        #endregion
    }
}
