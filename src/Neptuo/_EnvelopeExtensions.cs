using Neptuo.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// Common extensions for <see cref="Envelope{T}"/>.
    /// </summary>
    public static class _EnvelopeExtensions
    {
        #region TimeToLive

        /// <summary>
        /// Sets 'TimeToLive' to the metadata of the <paramref name="envelope"/>.
        /// </summary>
        /// <typeparam name="T">The type of the envelope body.</typeparam>
        /// <param name="envelope">The envelope which metadata to extend.</param>
        /// <param name="value">The vale of the 'TimeToLive'.</param>
        /// <returns><paramref name="envolope"/>(for fluency).</returns>
        public static Envelope<T> AddTimeToLive<T>(this Envelope<T> envelope, TimeSpan value)
        {
            Ensure.NotNull(envelope, "envelope");
            envelope.Metadata.Add("TimeToLive", value);
            return envelope;
        }

        /// <summary>
        /// Tries to read 'TimeToLive' from the metadata of the <paramref name="envelope"/>.
        /// </summary>
        /// <typeparam name="T">The type of the envelope body.</typeparam>
        /// <param name="envelope">The envelope to read the metadata from.</param>
        /// <param name="value">The value of the 'TimeToLive'.</param>
        /// <returns><c>true</c> if 'TimeToLive' can is contained in the <paramref name="envelope"/>; <c>false</c> otherwise.</returns>
        public static bool TryGetTimeToLive<T>(this Envelope<T> envelope, out TimeSpan value)
        {
            Ensure.NotNull(envelope, "envelope");
            return envelope.Metadata.TryGet("TimeToLive", out value);
        }

        /// <summary>
        /// Reads 'TimeToLive' from the metadata of the <paramref name="envelope"/> or throws <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <typeparam name="T">The type of the envelope body.</typeparam>
        /// <param name="envelope">The envelope to read the metadata from.</param>
        /// <returns>The value associated with the 'TimeToLive' or throws <see cref="InvalidOperationException"/>.</returns>
        /// <exception cref="InvalidOperationException">When the 'TimeToLive' is not contained in the metadata of the <paramref name="envelope"/>.</exception>
        public static TimeSpan GetTimeToLive<T>(this Envelope<T> envelope)
        {
            Ensure.NotNull(envelope, "envelope");
            return envelope.Metadata.Get<TimeSpan>("TimeToLive");
        }

        #endregion

        #region Delay

        /// <summary>
        /// Sets 'Delay' to the metadata of the <paramref name="envelope"/>.
        /// </summary>
        /// <typeparam name="T">The type of the envelope body.</typeparam>
        /// <param name="envelope">The envelope which metadata to extend.</param>
        /// <param name="value">The vale of the 'Delay'.</param>
        /// <returns><paramref name="envolope"/>(for fluency).</returns>
        public static Envelope<T> AddDelay<T>(this Envelope<T> envelope, TimeSpan value)
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
        public static bool TryGetDelay<T>(this Envelope<T> envelope, out TimeSpan value)
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
        public static TimeSpan GetDelay<T>(this Envelope<T> envelope)
        {
            Ensure.NotNull(envelope, "envelope");
            return envelope.Metadata.Get<TimeSpan>("Delay");
        }

        #endregion

        #region SourceID

        /// <summary>
        /// Sets 'SourceID' to the metadata of the <paramref name="envelope"/>.
        /// </summary>
        /// <typeparam name="T">The type of the envelope body.</typeparam>
        /// <param name="envelope">The envelope which metadata to extend.</param>
        /// <param name="value">The vale of the 'SourceID'.</param>
        /// <returns><paramref name="envolope"/>(for fluency).</returns>
        public static Envelope<T> AddSourceID<T>(this Envelope<T> envelope, string value)
        {
            Ensure.NotNull(envelope, "envelope");
            envelope.Metadata.Add("SourceID", value);
            return envelope;
        }

        /// <summary>
        /// Tries to read 'SourceID' from the metadata of the <paramref name="envelope"/>.
        /// </summary>
        /// <typeparam name="T">The type of the envelope body.</typeparam>
        /// <param name="envelope">The envelope to read the metadata from.</param>
        /// <param name="value">The value of the 'SourceID'.</param>
        /// <returns><c>true</c> if 'SourceID' can is contained in the <paramref name="envelope"/>; <c>false</c> otherwise.</returns>
        public static bool TryGetSourceID<T>(this Envelope<T> envelope, out string value)
        {
            Ensure.NotNull(envelope, "envelope");
            return envelope.Metadata.TryGet("SourceID", out value);
        }

        /// <summary>
        /// Reads 'SourceID' from the metadata of the <paramref name="envelope"/> or throws <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <typeparam name="T">The type of the envelope body.</typeparam>
        /// <param name="envelope">The envelope to read the metadata from.</param>
        /// <returns>The value associated with the 'SourceID' or throws <see cref="InvalidOperationException"/>.</returns>
        /// <exception cref="InvalidOperationException">When the 'SourceID' is not contained in the metadata of the <paramref name="envelope"/>.</exception>
        public static string GetSourceID<T>(this Envelope<T> envelope)
        {
            Ensure.NotNull(envelope, "envelope");
            return envelope.Metadata.Get<string>("SourceID");
        }

        #endregion
    }
}
